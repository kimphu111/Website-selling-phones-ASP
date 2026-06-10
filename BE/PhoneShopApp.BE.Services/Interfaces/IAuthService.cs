using PhoneShopApp.BE.Core.DTOs;

namespace PhoneShopApp.BE.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
