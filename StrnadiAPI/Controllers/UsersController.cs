using Microsoft.AspNetCore.Mvc;
using StrnadiAPI.Data.Models.Database;
using StrnadiAPI.Data.Models.Server;
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

    [HttpPost("/users/login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        LoginResult result = _repository.TryLogin(loginRequest.Email, loginRequest.Password);
        
        if (result == LoginResult.Success)
        {
            return Ok(new {Token = _jwtService.GenerateToken(loginRequest.Email)});
        }

        return Unauthorized();
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] User user)
    {
        if (user.Email == null!) 
            return BadRequest();

        // user.Password = user.Password.Sha256();
        
        AddResult result = _repository.Add(user);
        
        if (result == AddResult.Success)
        {
            Logger.Log($"User '{user.Email}' registered successfully.");
            Logger.Log($"Sending verification email to : '{user.Email}'");
            
            _emailSender.SendVerificationMessage(HttpContext, user.Email, user.Id);
            
            return Ok();
        }

        return BadRequest();
    }

    [HttpPut("/users/verifyEmail")]
    public IActionResult VerifyEmail([FromQuery] string jwt)
    {
        Logger.Log($"Trying to verify email with JWT");
        
#pragma warning disable CS8600 CS8604 // In the inside of if block the parsedEmail will be always true
        if (_jwtService.ValidateToken(jwt, out string parsedEmail))
        {
            _repository.ConfirmEmail(parsedEmail);
            Logger.Log($"Email {parsedEmail} verified successfully.");
            return Ok();
        }
#pragma warning restore CS8600 //
        
        return Unauthorized();
    }
    
    [HttpGet("/users/{id:int}")]
    public IActionResult Get(int id)
    {
        User? user = _repository.Get(id);
        
        Logger.Log($"Tried to get user with id: {id}.");
        
        return user is null ?
            NotFound() :
            Ok(user);
    }
}