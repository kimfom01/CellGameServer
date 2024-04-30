using System.Text.Json.Serialization;

namespace CellGameServer.Models;

public class Game
{
    [JsonPropertyName("id")] public Guid Id { get; set; }
    [JsonPropertyName("cells")] public int Cells { get; set; }

    [JsonPropertyName("clients")] public List<Client> Clients { get; set; } = [];
    [JsonPropertyName("state")] public Dictionary<int, string> State { get; set; } = new();
}