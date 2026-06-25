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
        Console.WriteLine($"[DEBUG-LOGIN] Đang tìm user: '{request.Username}'");
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        
        if (user is null)
        {
            Console.WriteLine($"[DEBUG-LOGIN] THẤT BẠI: Không tìm thấy user '{request.Username}' trong Database!");
            return null;
        }

        Console.WriteLine($"[DEBUG-LOGIN] Tìm thấy user. PasswordHash trong DB là: '{user.PasswordHash}'");
        
        var inputHash = PasswordHasher.Hash(request.Password);
        Console.WriteLine($"[DEBUG-LOGIN] PasswordHash người dùng nhập là: '{inputHash}'");

        if (!string.Equals(inputHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("[DEBUG-LOGIN] THẤT BẠI: Hai chuỗi Hash không khớp nhau!");
            return null;
        }

        Console.WriteLine("[DEBUG-LOGIN] THÀNH CÔNG: Mật khẩu đúng, đang tạo Token...");
        return JwtTokenGeneratorSingleton.Instance.GenerateToken(user, _configuration);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AUTH ERROR] Database issue: {ex.Message}");
        throw; 
    }
}
}
