using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneShopApp.BE.Infrastructure.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _dbContext;

    public AppUserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<List<AppUser>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.OrderBy(x => x.Id).ToListAsync(cancellationToken);
    }

    public async Task<AppUser> AddAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return user;
    }
}
