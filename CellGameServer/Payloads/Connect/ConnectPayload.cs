using System.Text.Json.Serialization;

namespace CellGameServer.Payloads.Connect;

public sealed class ConnectPayload : PayloadBase
{
    [JsonPropertyName("clientId")]
    public Guid ClientId { get; set; }
}