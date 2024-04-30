using System.Net;
using System.Text.Json;
using CellGameServer.Payloads.Play;

namespace CellGameServer.MethodChains;

public class PlayMethodHandler : MethodHandler
{
    public override async Task HandleRequest(MethodRequestModel request, CancellationToken cancellationToken)
    {
        if (request.Method == "play")
        {
            var playPayload = JsonSerializer.Deserialize<PlayPayload>(request.Message);

            if (playPayload is null)
            {
                request.Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            request.Manager.PlayGame(playPayload, cancellationToken);
        }
        else if (Successor is not null)
        {
            await Successor.HandleRequest(request, cancellationToken);
        }
    }
}