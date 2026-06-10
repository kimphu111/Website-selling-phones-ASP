using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PhoneShopApp.BE.Infrastructure.Repositories;

public class PhoneRepository : IPhoneRepository
{
    private readonly AppDbContext _dbContext;

    public PhoneRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Phone>> ListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Phones.OrderByDescending(x => x.Id).ToListAsync(cancellationToken);
    }

    public Task<Phone?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Phones.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Phone> AddAsync(Phone phone, CancellationToken cancellationToken = default)
    {
        _dbContext.Phones.Add(phone);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return phone;
    }

    public async Task<Phone> UpdateAsync(Phone phone, CancellationToken cancellationToken = default)
    {
        _dbContext.Phones.Update(phone);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return phone;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var phone = await _dbContext.Phones.FindAsync(new object[] { id }, cancellationToken);
        if (phone is not null)
        {
            _dbContext.Phones.Remove(phone);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
