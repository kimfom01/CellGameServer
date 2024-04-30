using System.Text.Json.Serialization;

namespace CellGameServer.Payloads;

public class PayloadBase
{
    [JsonPropertyName("method")]
    public string Method { get; set; }
}