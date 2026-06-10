using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace PhoneShopApp.BE.Services.Patterns.Singleton;

// Singleton Pattern: một instance duy nhất để tạo JWT token trong toàn ứng dụng.
public sealed class JwtTokenGeneratorSingleton
{
    private static readonly Lazy<JwtTokenGeneratorSingleton> LazyInstance =
        new(() => new JwtTokenGeneratorSingleton());

    public static JwtTokenGeneratorSingleton Instance => LazyInstance.Value;

    private JwtTokenGeneratorSingleton()
    {
    }

    public LoginResponseDto GenerateToken(AppUser user, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
        var issuer = configuration["Jwt:Issuer"] ?? "PhoneShopApp";
        var audience = configuration["Jwt:Audience"] ?? "PhoneShopAppClients";
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var value) ? value : 120;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Role = user.Role,
            ExpiresAtUtc = expiresAt
        };
    }
}
