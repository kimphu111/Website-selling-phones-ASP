using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Core.Interfaces.Repositories;

public interface IAppUserRepository
{
    Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<List<AppUser>> ListAsync(CancellationToken cancellationToken = default);
    Task<AppUser> AddAsync(AppUser user, CancellationToken cancellationToken = default);
}
