using System.Net;
using System.Text.Json;
using CellGameServer.Payloads.Create;

namespace CellGameServer.MethodChains;

public class CreateMethodHandler : MethodHandler
{
    public override async Task HandleRequest(MethodRequestModel request, CancellationToken cancellationToken)
    {
        if (request.Method == "create")
        {
            var createPayloadRequest = JsonSerializer.Deserialize<CreatePayloadRequest>(request.Message);

            if (createPayloadRequest is null)
            {
                request.Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            await request.Manager.CreateGame(createPayloadRequest, cancellationToken);
        }
        else if (Successor is not null)
        {
            await Successor.HandleRequest(request, cancellationToken);
        }
    }
}