using System.Net.WebSockets;

namespace StrnadiAPI.Services;

public class WsHandler
{
    private WebSocket _webSocket;

    public WsHandler(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    public async Task<int?> ReadIntAsync()
    {
        byte[] result = await ReadBytesAsync(sizeof(int));
        
        if (result.Length == sizeof(int))
        {
            return BitConverter.ToInt32(result, 0);
        }
    
        return null;
    }

    public async Task<byte[]> ReadBytesAsync(int count)
    {
        byte[] buffer = new byte[count];
        
        WebSocketReceiveResult result = await _webSocket.ReceiveAsync(buffer, CancellationToken.None);

        return buffer;
    }
}