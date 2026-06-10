using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Core.Interfaces.Repositories;

public interface IPhoneRepository
{
    Task<List<Phone>> ListAsync(CancellationToken cancellationToken = default);
    Task<Phone?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Phone> AddAsync(Phone phone, CancellationToken cancellationToken = default);
    Task<Phone> UpdateAsync(Phone phone, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
