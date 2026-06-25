using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Services.Interfaces;

namespace PhoneShopApp.BE.Services.Patterns.Facade;

// Facade Pattern: gom nhiều service nghiệp vụ thành một cổng dùng cho API.
public class PhoneShopFacade : IPhoneShopFacade
{
    private readonly IUserService _userService;
    private readonly IPhoneService _phoneService;

    public PhoneShopFacade(IUserService userService, IPhoneService phoneService)
    {
        _userService = userService;
        _phoneService = phoneService;
    }

    public Task<List<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return _userService.ListAsync(cancellationToken);
    }

    public Task<UserDto> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        return _userService.CreateAsync(dto, cancellationToken);
    }

    public Task<List<PhoneDto>> SearchPhonesAsync(string? keyword, string? searchBy, 
    CancellationToken cancellationToken = default)
    {
        return _phoneService.SearchAsync(keyword, searchBy, cancellationToken);
    }

    public Task<PhoneDto> CreatePhoneAsync(CreatePhoneDto dto, CancellationToken cancellationToken = default)
    {
        return _phoneService.CreateAsync(dto, cancellationToken);
    }

    public Task<PhoneDto> UpdatePhoneAsync(int id, UpdatePhoneDto dto, CancellationToken cancellationToken = default)
    {
        return _phoneService.UpdateAsync(id, dto, cancellationToken);
    }

    public Task<PhoneDto> UpdatePhoneStatusAsync(int id, string action, CancellationToken cancellationToken = default)
    {
        return _phoneService.UpdateStatusAsync(id, action, cancellationToken);
    }

    public Task DeletePhoneAsync(int id, CancellationToken cancellationToken = default)
    {
        return _phoneService.DeleteAsync(id, cancellationToken);
    }
}
