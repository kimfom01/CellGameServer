using System.Text.Json.Serialization;

namespace CellGameServer.Payloads.Create;

public class CreatePayloadRequest : PayloadBase
{
    [JsonPropertyName("clientId")]
    public Guid ClientId { get; set; }
}