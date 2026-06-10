using PhoneShopApp.BE.Core.DTOs;

namespace PhoneShopApp.BE.Services.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> ListAsync(CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
}
