using System.Net.WebSockets;
using CellGameServer.Services;

namespace CellGameServer.MethodChains;

public class MethodRequestModel
{
    public string Method { get; set; }
    public string Message { get; set; }
    public HttpContext Context { get; set; }
    public IManager Manager { get; set; }
    public WebSocket WebSocket { get; set; }
}