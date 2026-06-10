using PhoneShopApp.BE.Core.DTOs;

namespace PhoneShopApp.BE.Services.Interfaces;

// Facade Pattern: Gom nhóm các service nghiệp vụ vào một cổng duy nhất cho Controllers.
public interface IPhoneShopFacade
{
    Task<List<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<List<PhoneDto>> SearchPhonesAsync(string? keyword, string? searchBy, CancellationToken cancellationToken = default);
    Task<PhoneDto> CreatePhoneAsync(CreatePhoneDto dto, CancellationToken cancellationToken = default);
    Task<PhoneDto> UpdatePhoneAsync(int id, UpdatePhoneDto dto, CancellationToken cancellationToken = default);
    Task<PhoneDto> UpdatePhoneStatusAsync(int id, string action, CancellationToken cancellationToken = default);
    Task DeletePhoneAsync(int id, CancellationToken cancellationToken = default);
}
