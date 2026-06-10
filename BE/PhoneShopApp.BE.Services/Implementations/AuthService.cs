using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Services.Interfaces;
using PhoneShopApp.BE.Services.Patterns.Singleton;
using Microsoft.Extensions.Configuration;

namespace PhoneShopApp.BE.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IAppUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAppUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (user is null)
            {
                return null;
            }

            var inputHash = PasswordHasher.Hash(request.Password);
            if (!string.Equals(inputHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return JwtTokenGeneratorSingleton.Instance.GenerateToken(user, _configuration);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH ERROR] Database issue: {ex.Message}");
            throw; 
        }
    }
}
