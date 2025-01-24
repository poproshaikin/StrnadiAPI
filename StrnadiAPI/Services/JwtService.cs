using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Services;

public class JwtService
{
    private IConfiguration _configuration;
    
    private string _secretKey => _configuration["Jwt:SecretKey"];
    
    private string _issuer => _configuration["Jwt:Issuer"];
    
    private string _audience => _configuration["Jwt:Audience"];
    
    private string _lifetime => _configuration["Jwt:Lifetime"];
    
    private TimeSpan _lifetimeAsTimeSpan => TimeSpan.Parse(_lifetime);
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string userId)
    {
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _audience),
            new Claim(JwtRegisteredClaimNames.Exp, _lifetime)
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

        return tokenHandler.WriteToken(token);
    }

    public bool TryParseUserId(string token, out string? userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = key
        };

        try
        {
            ClaimsPrincipal? claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            userId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (Exception)
        {
            userId = null;
        }

        return userId is not null;
    }
}