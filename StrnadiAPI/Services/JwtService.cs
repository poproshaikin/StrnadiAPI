using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Services;

public class JwtService
{
    private IConfiguration _configuration;
    
    private string _secretKey => _configuration["Authentication:JwtSecretKey"];
    
    private string _issuer => _configuration["Authentication:JwtIssuer"];
    
    private string _audience => _configuration["Authentication:JwtAudience"];
    
    private string _lifetime => _configuration["Authentication:JwtLifetime"];
    
    private TimeSpan _lifetimeAsTimeSpan => TimeSpan.Parse(_lifetime);
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string email)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iss, _issuer),
            new(JwtRegisteredClaimNames.Aud, _audience),
            new(JwtRegisteredClaimNames.Exp, _lifetime)
        ];
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_lifetimeAsTimeSpan),
            SigningCredentials = creds,
            Issuer = _issuer,
            Audience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        string jwtToken = tokenHandler.WriteToken(token);
        
        Logger.Log($"Generated token for user {email} : {jwtToken}");

        return jwtToken;
    }

    public bool TryParseEmail(string token, out string? email)
    {
        Logger.Log($"Trying to authorize with token {token}");
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = key
        };

        try
        {
            ClaimsPrincipal? claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            email = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (Exception)
        {
            email = null;
        }
        
        Logger.Log(email is null ? "Authorization failed" : $"Authorization successfull : {email}");
        return email is not null;
    }
}