using System.Text.Json.Serialization;
using CellGameServer.Models;

namespace CellGameServer.Payloads.Join;

public class JoinPayloadResponse : PayloadBase
{
    [JsonPropertyName("game")]
    public Game Game { get; set; }
}