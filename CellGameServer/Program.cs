using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CellGameServer.MethodChains;
using CellGameServer.Payloads;
using CellGameServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:9000");
builder.Services.AddSingleton<IManager, Manager>();

var app = builder.Build();

app.UseWebSockets();

app.Map("/", async (IManager manager, CancellationToken cancellationToken, HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        await manager.ConnectNewClient(webSocket, cancellationToken);

        await manager.ReceiveMessage(webSocket, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                var payload = JsonSerializer.Deserialize<PayloadBase>(message);

                if (payload is null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                MethodHandler create = new CreateMethodHandler();
                MethodHandler join = new JoinMethodHandler();
                MethodHandler play = new PlayMethodHandler();

                create.SetSuccessor(join);
                join.SetSuccessor(play);

                await create.HandleRequest(new MethodRequestModel
                {
                    Context = context,
                    Manager = manager,
                    Message = message,
                    Method = payload.Method,
                    WebSocket = webSocket
                }, cancellationToken);
            }
            else if (result.MessageType == WebSocketMessageType.Close
                     || webSocket.State == WebSocketState.Aborted)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellationToken);
            }
        }, cancellationToken);
    }
});

app.Run();