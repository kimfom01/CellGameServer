using System.Net;
using System.Text.Json;
using CellGameServer.Payloads.Join;

namespace CellGameServer.MethodChains;

public class JoinMethodHandler : MethodHandler
{
    public override async Task HandleRequest(MethodRequestModel request, CancellationToken cancellationToken)
    {
        if (request.Method == "join")
        {
            var joinPayloadRequest = JsonSerializer.Deserialize<JoinPayloadRequest>(request.Message);

            if (joinPayloadRequest is null)
            {
                request.Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            await request.Manager.JoinGame(request.WebSocket, joinPayloadRequest, cancellationToken);
        }
        else if (Successor is not null)
        {
            await Successor.HandleRequest(request, cancellationToken);
        }
    }
}