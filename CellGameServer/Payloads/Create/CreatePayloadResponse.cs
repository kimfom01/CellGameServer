using System.Text.Json.Serialization;
using CellGameServer.Models;

namespace CellGameServer.Payloads.Create;

public class CreatePayloadResponse : PayloadBase
{
    [JsonPropertyName("game")]
    public Game Game { get; set; }
}