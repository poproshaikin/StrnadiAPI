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
    private DateTime _expiresAt => DateTime.UtcNow.Add(_lifetimeAsTimeSpan);
    
    private const string security_algorithm = SecurityAlgorithms.HmacSha256;

    private readonly SecurityKey _securityKey;
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    }

    public string GenerateToken(string email)
    {
        var claims = new[]
        {
            new Claim("sub", email),
        };
        var creds = new SigningCredentials(_securityKey, security_algorithm);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _expiresAt,
            SigningCredentials = creds,
            Issuer = _issuer,
            Audience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token, out string? email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, 
            IssuerSigningKey = _securityKey,
            RequireExpirationTime = true
        };

        try
        {
            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            foreach (Claim claim in claimsPrincipal.Claims)
            {
                Console.WriteLine(claim.Type + ": " + claim.Value);
            }
            
            email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            return email is not null;
        }
        catch (Exception ex)
        {
            email = null;
            Logger.Log($"Token validation failed: {ex.Message}");
            return false;
        }
    }
}