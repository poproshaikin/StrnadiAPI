using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordingsController : ControllerBase
{
    // dostat overene/neoverene nahravky konkretniho usera
    // 

    private IRecordingsRepository _repo;
    
    public RecordingsController(IRecordingsRepository recordingsRepository)
    {   
        _repo = recordingsRepository;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_repo.Get());
    }

    [HttpPost("[controller]/uploadRec")]
    public IActionResult UploadRec([FromBody] Recording recording)
    {
        AddResult result = _repo.Add(recording, returningProperty: rec => rec.Id, out int generatedId);

        if (result == AddResult.Success)
        {
            return Ok(generatedId);
        }
        
        return BadRequest();
    }

    [HttpPost("[controller]/updateRecPart")]
    public IActionResult UpdateRecPart([FromBody] RecordingPart recordingPart)
    {
        
    }
}