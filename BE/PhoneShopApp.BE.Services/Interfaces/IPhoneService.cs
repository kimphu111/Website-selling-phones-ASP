using PhoneShopApp.BE.Core.DTOs;

namespace PhoneShopApp.BE.Services.Interfaces;

public interface IPhoneService
{
    Task<List<PhoneDto>> SearchAsync(string? keyword, string? searchBy, CancellationToken cancellationToken = default);
    Task<PhoneDto> CreateAsync(CreatePhoneDto dto, CancellationToken cancellationToken = default);
    Task<PhoneDto> UpdateAsync(int id, UpdatePhoneDto dto, CancellationToken cancellationToken = default);
    Task<PhoneDto> UpdateStatusAsync(int id, string action, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
