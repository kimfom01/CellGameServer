using System.Text.Json.Serialization;

namespace CellGameServer.Payloads.Play;

public class PlayPayload
{
    [JsonPropertyName("gameId")] public Guid GameId { get; set; }
    [JsonPropertyName("clientId")] public Guid ClientId { get; set; }
    [JsonPropertyName("cellId")] public int CellId { get; set; }
    [JsonPropertyName("playerColor")] public string PlayerColor { get; set; }
}