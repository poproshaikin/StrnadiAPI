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
        return Ok(_repo.GetAll());
    }

    [HttpPost]  // overeni
    public IActionResult UploadNew([FromBody] Recording recording)
    {
        
    }
    
    [HttpPut]  // overeni
    public IActionResult Put([FromBody] Recording recording)
    {
        
    }

    [HttpDelete("[controller]/{id:int}")] // overeni
    public IActionResult Delete([FromQuery] int id)
    {
        
    }
}