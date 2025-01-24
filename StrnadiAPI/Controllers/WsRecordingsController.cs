using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;
using StrnadiAPI.Services;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class WsRecordingsController : ControllerBase
{
    private FileSystemHelper _fsHelper;
    private IRecordingPartsRepository _repo;

    public WsRecordingsController(IRecordingPartsRepository repo)
    {
        _fsHelper = new FileSystemHelper();
        _repo = repo;
    }
    
    [Route("[controller]/uploadRecPart")]
    public async Task<IActionResult> Upload()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            RecordingPartDto recordingPart = await ProcessConnectionAsync(webSocket);

            
        }
        else
        {
            return BadRequest();
        }
    }

    private async Task<RecordingPartDto> ProcessConnectionAsync(WebSocket webSocket)
    {
        MemoryStream data = await ReceiveDataAsync(webSocket);
        StreamHelper streamHelper = new StreamHelper(data);

        int dataLength = await streamHelper.ReadIntAsync();
        int recordingPartId = await streamHelper.ReadIntAsync();
        byte[] recordingPartData = await streamHelper.ReadBytesAsync(dataLength);

        if (!_repo.Exists(recordingPartId))
        {
            
        }

        int recordingId = _repo.GetById(recordingPartId)!.RecordingId;
        
        _fsHelper.SaveRecordingSoundFile(recordingId, recordingPartId, recordingPartData);
    }

    private async Task<MemoryStream> ReceiveDataAsync(WebSocket webSocket)
    {
        byte[] buffer = new byte[8096];
        MemoryStream stream = new();

        while (true)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            
            stream.Write(buffer, 0, result.Count);

            if (result.EndOfMessage)
            {
                break;
            }
        }

        return stream;
    }
}