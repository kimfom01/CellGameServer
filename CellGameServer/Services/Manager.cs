using System.Net.WebSockets;
using System.Text.Json;
using CellGameServer.Models;
using CellGameServer.Payloads.Connect;
using CellGameServer.Payloads.Create;
using CellGameServer.Payloads.Join;
using CellGameServer.Payloads.Play;
using CellGameServer.Payloads.Update;

namespace CellGameServer.Services;

public class Manager : IManager
{
    private readonly ILogger<Manager> _logger;
    private readonly Dictionary<Guid, Game> _games = new();

    private readonly Dictionary<Guid, WebSocket> _clientConnections = new();

    public Manager(ILogger<Manager> logger)
    {
        _logger = logger;
    }

    public async Task CreateGame(CreatePayloadRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new game");
        var gameId = Guid.NewGuid();
        _games.Add(gameId, new Game
        {
            Id = gameId,
            Cells = 20
        });

        var payload = new CreatePayloadResponse
        {
            Method = "create",
            Game = _games[gameId],
        };

        var webSocket = _clientConnections[request.ClientId];

        await SendPayload(webSocket, payload, cancellationToken);
        _logger.LogInformation("New game created id={gameId}", gameId);
    }

    public async Task ConnectNewClient(WebSocket webSocket, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Connecting new client");
        var clientId = Guid.NewGuid();
        _clientConnections.Add(clientId, webSocket);

        await SendPayload(webSocket, new ConnectPayload
        {
            Method = "connect",
            ClientId = clientId
        }, cancellationToken);
        _logger.LogInformation("New client connected id={clientId}", clientId);
    }

    public async Task ReceiveMessage(WebSocket socket, Func<WebSocketReceiveResult, byte[], Task> handleMessage,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receiving new message");
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            _logger.LogInformation("Handling received message");
            await handleMessage(result, buffer);
        }
    }

    public async Task JoinGame(WebSocket socket, JoinPayloadRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Joining game id={gameId}", request.GameId);
        var clientId = request.ClientId;
        var gameId = request.GameId;

        var game = _games[gameId];

        if (game.Clients.Count == 2)
        {
            return;
        }

        var color = new Dictionary<int, string>
        {
            [0] = "Red", [1] = "Green",
        }[game.Clients.Count];

        game.Clients.Add(new Client
        {
            Id = clientId,
            Color = color
        });

        foreach (var cli in game.Clients)
        {
            await SendPayload(_clientConnections[cli.Id], new JoinPayloadResponse
            {
                Method = "join",
                Game = game
            }, cancellationToken);
        }

        _logger.LogInformation("Client id={clientId} joined game id={gameId}", request.ClientId, request.GameId);

        if (game.Clients.Count == 2)
        {
            await UpdateGameState(cancellationToken);

            await SetInterval(UpdateGameState, TimeSpan.FromSeconds(0.1), cancellationToken);
        }
    }

    private async Task UpdateGameState(CancellationToken cancellationToken)
    {
        foreach (var (id, game) in _games)
        {
            _logger.LogInformation("Updating clients in game id={gameId}", id);
            foreach (var cli in game.Clients)
            {
                await SendPayload(_clientConnections[cli.Id], new UpdatePayload
                {
                    Game = game, 
                    Method = "update"
                }, cancellationToken);
            }
        }
    }

    public void PlayGame(PlayPayload request, CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            var state = _games[request.GameId].State;

            state[request.CellId] = request.PlayerColor;
            _games[request.GameId].State = state;
        }
    }

    private async Task SendPayload<T>(WebSocket webSocket, T payload, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending payload");
        var bytes = JsonSerializer.SerializeToUtf8Bytes(payload);
        var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
        await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, cancellationToken);
        _logger.LogInformation("Payload sent");
    }

    private async Task SetInterval(Func<CancellationToken, Task> func, TimeSpan timeSpan,
        CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(timeSpan, cancellationToken);

            await func(cancellationToken);

            await SetInterval(func, timeSpan, cancellationToken);
        }
    }
}