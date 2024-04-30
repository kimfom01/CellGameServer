using System.Text.Json.Serialization;

namespace CellGameServer.Models;

public class Client
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("color")] public string Color { get; set; }
}