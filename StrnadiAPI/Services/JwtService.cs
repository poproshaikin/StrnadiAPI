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
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Iss, _issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _audience),
            ]),
            SigningCredentials = new SigningCredentials(_securityKey, security_algorithm),
            Expires = _expiresAt
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
        
        // var claims = new[]
        // {
        //     new Claim("sub", email),
        // };
        // var creds = new SigningCredentials(_securityKey, security_algorithm);
        //
        // var tokenDescriptor = new SecurityTokenDescriptor
        // {
        //     Subject = new ClaimsIdentity(claims),
        //     Expires = _expiresAt,
        //     SigningCredentials = creds,
        //     Issuer = _issuer,
        //     Audience = _audience
        // };
        //
        // var tokenHandler = new JwtSecurityTokenHandler();
        // var token = tokenHandler.CreateToken(tokenDescriptor);
        // return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token, out string? email)
    {
        if (Validate(token))
        {
            email = GetEmail(token);

            if (email is null)
            {
                Logger.Log("Failed to read email from validated token.");
                return false;
            }

            return true;
        }
        
        Logger.Log("Failed to validate JWT token.");

        email = null;
        return false;
    }

    private bool Validate(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _securityKey,
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch (SecurityTokenException ex)
        {
            Logger.Log($"Failed to validate JWT token: {ex.Message}");
            return false;
        }
        catch (Exception e)
        {
            Logger.Log($"An exception thrown during JWT token validation: {e.Message}");
            return false;
        }
    }

    private Claim[] GetClaims(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken? decodedToken = tokenHandler.ReadJwtToken(token);
        
        return decodedToken.Claims.ToArray();
    }
    
    private string? GetEmail(string token)
    {
        return GetClaims(token).FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
    }
}