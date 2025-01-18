using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;
using StrnadiAPI.Services;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("/users/")]
public class UsersController : ControllerBase
{
    private IUsersRepository _repository;
    private StrnadiEmailVerifier _verifier;
    private StrnadiLinkGenerator _generator;
    
    public UsersController(IConfiguration configuration, IUsersRepository repository)
    {
        _repository = repository;
        _generator = new StrnadiLinkGenerator();
        _verifier = new StrnadiEmailVerifier(configuration);
    }

    [HttpGet]
    public IActionResult Get([FromBody] string jwt)
    {
        
    }
    
    //
    // [HttpGet]
    // public IActionResult Get([FromBody] string orderingJson)
    // {
    //     IReadOnlyList<User> orderedUsers = _repository.GetAndOrder(orderingJson);
    //
    //     return Ok(orderedUsers);
    // }
    //
    // [HttpGet("/{id:int}")]
    // public IActionResult Get(int id)
    // {
    //     User? user = _repository.Get(id);
    //     
    //     return user is null ?
    //         NotFound() :
    //         Ok(user);
    // }
    //
    // // reset password
    //
    // [HttpPost]
    // public IActionResult UploadNew([FromBody] User user)
    // {
    //     try
    //     {
    //         _verifier.SendLink(user.Email, );
    //         
    //         _repository.Add(user);
    //         
    //         return Ok();
    //     }
    //     catch
    //     {
    //         return BadRequest();
    //     }
    // }
    //
    // [HttpPut]
    // public IActionResult Put([FromBody] string changesJson)
    // {
    //     UpdateResult result = _repository.Update(changesJson);
    //     
    //     return result switch
    //     {
    //         UpdateResult.Successful => Ok(),
    //         UpdateResult.NotFound => NotFound(),
    //         UpdateResult.Fail => BadRequest(),
    //         UpdateResult.IdNotProvided => BadRequest(),
    //         UpdateResult.WrongIdProvided => BadRequest(),
    //         UpdateResult.UniquenessViolation => Conflict(),
    //         
    //         _ => BadRequest()
    //     };
    // }
    //
    // [HttpDelete("[controller]/{id:int}")]
    // public IActionResult Delete(int id)
    // {
    //     bool success = _repository.Delete(id);
    //     
    //     return success ? Ok() : NotFound();
    // }
}