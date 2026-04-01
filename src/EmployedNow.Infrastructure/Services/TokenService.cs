using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployedNow.Application.Interfaces;
using EmployedNow.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EmployedNow.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Creates a signed JWT containing identity and role claims.
    /// </summary>
    public string GenerateToken(User user)
    {
        var issuer = _configuration["Jwt:Issuer"] ?? "EmployedNow";
        var audience = _configuration["Jwt:Audience"] ?? "EmployedNowClients";
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");

        // Keeps token payload intentionally lightweight for MVP.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
