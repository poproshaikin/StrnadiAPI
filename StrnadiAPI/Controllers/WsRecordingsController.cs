using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;
using StrnadiAPI.Services;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("/ws/")]
public class WsRecordingsController : ControllerBase
{
    private readonly FileSystemHelper _fsHelper;
    private readonly IRecordingsRepository _recRepo;
    private readonly IRecordingPartsRepository _recPartsRepo;

    public WsRecordingsController(IRecordingsRepository recRepo, IRecordingPartsRepository recPartsRepo)
    {
        _fsHelper = new FileSystemHelper();
        _recRepo = recRepo;
        _recPartsRepo = recPartsRepo;
    }

    [Route("/ws/rec/upload")]
    public async Task<IActionResult> Upload()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest) 
            return BadRequest();
        
        WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

        var result = await ReceivePart(webSocket);

        if (result.Error is not null)
            return StatusCode(500, result.Error);

        if (!_recRepo.Exists(result.RecordingId) || !_recPartsRepo.Exists(result.Id))
            return StatusCode(500, UploadError.DoesntExist);

        string path = _fsHelper.SaveRecordingSoundFile(result.Id,
            result.RecordingId,
            result.Data!);
            
        RecordingPart part = _recPartsRepo.GetById(result.Id)!;
        part.FilePath = path;
        _recPartsRepo.Update(part);

        return Ok();

    }

    private async Task<UploadResult> ReceivePart(WebSocket socket)
    {
        WsHandler handler = new(socket);

        int? dataLength = await handler.ReadIntAsync();
        int? recordingPartId = await handler.ReadIntAsync();

        if (dataLength is null || recordingPartId is null)
        {
            return new UploadResult(error: UploadError.StreamInterrupted);
        }
        
        byte[] data = await handler.ReadBytesAsync(dataLength.Value);
 
        if (data.Length != dataLength.Value)
        {
            return new UploadResult(id: recordingPartId.Value, error: UploadError.StreamInterrupted);
        }
        
        return new UploadResult(recordingPartId.Value, data);
    }
}