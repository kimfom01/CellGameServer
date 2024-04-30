using System.Text.Json.Serialization;
using CellGameServer.Models;

namespace CellGameServer.Payloads.Update;

public class UpdatePayload : PayloadBase
{
    [JsonPropertyName("game")]
    public Game Game { get; set; }
}