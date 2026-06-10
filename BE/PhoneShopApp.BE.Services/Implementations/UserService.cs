using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Services.Interfaces;

namespace PhoneShopApp.BE.Services.Implementations;

public class UserService : IUserService
{
    private readonly IAppUserRepository _userRepository;

    public UserService(IAppUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _userRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (exists is not null)
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var user = new AppUser
        {
            Username = dto.Username,
            FullName = dto.FullName,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            Role = string.IsNullOrWhiteSpace(dto.Role) ? "Member" : dto.Role
        };

        var created = await _userRepository.AddAsync(user, cancellationToken);
        return Map(created);
    }

    private static UserDto Map(AppUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
