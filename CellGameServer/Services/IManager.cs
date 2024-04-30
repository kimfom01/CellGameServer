using System.Net.WebSockets;
using CellGameServer.Payloads.Create;
using CellGameServer.Payloads.Join;
using CellGameServer.Payloads.Play;

namespace CellGameServer.Services;

public interface IManager
{
    Task CreateGame(CreatePayloadRequest request, CancellationToken cancellationToken);
    Task ConnectNewClient(WebSocket webSocket, CancellationToken cancellationToken);

    Task ReceiveMessage(WebSocket socket, Func<WebSocketReceiveResult, byte[], Task> handleMessage,
        CancellationToken cancellationToken);

    Task JoinGame(WebSocket socket, JoinPayloadRequest request, CancellationToken cancellationToken);
    void PlayGame(PlayPayload request, CancellationToken cancellationToken);
}