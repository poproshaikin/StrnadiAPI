using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("/recordings")]
public class RecordingsController : ControllerBase
{
    private readonly IRecordingsRepository _recRepo;
    private readonly IRecordingPartsRepository _recPartsRepo;
    
    private WsRecordingsController? _cachedWs;
    
    public RecordingsController(IRecordingsRepository recRepo, 
        IRecordingPartsRepository recPartsRepo)
    {   
        _recRepo = recRepo;
        _recPartsRepo = recPartsRepo;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_recRepo.Get());
    }

    [HttpPost("/recordings/uploadRec")]
    public IActionResult UploadRec([FromBody] Recording recording)
    {
        AddResult result = _recRepo.Add(recording, returning: rec => rec.Id, out int generatedId);
        
        if (result == AddResult.Fail)
        {
            return Conflict();
        }
        
        return Ok(generatedId);
    }

    [HttpPost("/recordings/updateRecPart")]
    public IActionResult UpdateRecPart([FromBody] RecordingPart recordingPart)
    {
        AddResult result = _recPartsRepo.Add(recordingPart, returning: recPart => recPart.Id, out int generatedId);

        if (result == AddResult.Fail)
        {
            return Conflict();
        }

        return Ok(generatedId);
    }
}