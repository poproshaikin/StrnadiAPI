using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Repositories;
using StrnadiAPI.Services;

namespace StrnadiAPI.Controllers;

[ApiController]
[Route("/users")]
public class UsersController : ControllerBase
{
    private IUsersRepository _repository;
    private EmailSender _emailSender;
    private JwtService _jwtService;
    
    public UsersController(IConfiguration configuration, IUsersRepository repository)
    {
        _repository = repository;
        _emailSender = new EmailSender(configuration);
        _jwtService = new JwtService(configuration);
    }

    [HttpGet("/login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        LoginResult result = _repository.TryLogin(loginDto.Email, loginDto.Password);

        if (result == LoginResult.Success)
        {
            return Ok(_jwtService.GenerateToken(loginDto.Email));
        }
        
        return Ok(result);
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] User user)
    {
        if (user.Email == null!) 
            return BadRequest();
        
        AddResult result = _repository.Add(user);
        
        if (result == AddResult.Success)
        {
            _emailSender.SendVerificationMessage(HttpContext, user.Email, user.Id);
            
            return Ok();
        }

        return BadRequest();
    }

    [HttpPut("/verifyEmail")]
    public IActionResult VerifyEmail([FromQuery] string userId, [FromQuery] string jwt)
    {
#pragma warning disable CS8600 // In the inside of if block the parsedUserId will be always true
        if (_jwtService.TryParseUserId(jwt, out string parsedUserId))
#pragma warning restore CS8600 //
        {
            if (userId != parsedUserId)
            {
                return Unauthorized();
            }

            _repository.ConfirmEmail(userId);
        }
        
        return Unauthorized();
    }
    
    [HttpGet("/{id:int}")]
    public IActionResult Get(int id)
    {
        User? user = _repository.Get(id);
        
        return user is null ?
            NotFound() :
            Ok(user);
    }
}