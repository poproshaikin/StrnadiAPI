using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StrnadiAPI.Data.Models.Database;

namespace StrnadiAPI.Services;

public class JwtService
{
    private IConfiguration _configuration;
    
    private string _secretKey => _configuration["Authentication:JwtSecretKey"] ?? throw new NullReferenceException("Invalid configuration key passed");
    
    private string _issuer => _configuration["Authentication:JwtIssuer"] ?? throw new NullReferenceException("Invalid configuration key passed");
    
    private string _audience => _configuration["Authentication:JwtAudience"] ?? throw new NullReferenceException("Invalid configuration key passed");
    
    private string _lifetime => _configuration["Authentication:JwtLifetime"] ?? throw new NullReferenceException("Invalid configuration key passed");
    
    private TimeSpan _lifetimeAsTimeSpan => TimeSpan.Parse(_lifetime);
    private DateTime _issuedAt => DateTime.UtcNow.Add(_lifetimeAsTimeSpan);
    
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
            new(JwtRegisteredClaimNames.Exp, new DateTimeOffset(_issuedAt).ToUnixTimeSeconds().ToString())
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

        return jwtToken;
    }

    public bool TryParseEmail(string token, out string? email)
    {
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            ClaimsPrincipal? claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out _);

            email = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine($"JWT validation failed: {ex.Message}");
            email = null;
        }
        
        return email is not null;
    }
}