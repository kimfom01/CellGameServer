using System.Text.Json.Serialization;

namespace CellGameServer.Payloads.Join;

public class JoinPayloadRequest : PayloadBase
{
    [JsonPropertyName("clientId")]
    public Guid ClientId { get; set; }
    [JsonPropertyName("gameId")]
    public Guid GameId { get; set; }
}