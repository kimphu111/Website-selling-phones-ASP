using PhoneShopApp.BE.Core.DTOs;
using PhoneShopApp.BE.Core.Entities;
using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Services.Interfaces;
using PhoneShopApp.BE.Services.Patterns.Factory;
using PhoneShopApp.BE.Services.Patterns.State;
using PhoneShopApp.BE.Services.Patterns.Iterator;

namespace PhoneShopApp.BE.Services.Implementations;

public class PhoneService : IPhoneService
{
    private readonly IPhoneRepository _phoneRepository;
    private readonly PhoneSearchFactoryMethod _searchFactory;

    public PhoneService(IPhoneRepository phoneRepository, PhoneSearchFactoryMethod searchFactory)
    {
        _phoneRepository = phoneRepository;
        _searchFactory = searchFactory;
    }

    public async Task<List<PhoneDto>> SearchAsync(string? keyword, string? searchBy, CancellationToken cancellationToken = default)
    {
        var phones = await _phoneRepository.ListAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var strategy = _searchFactory.Create(searchBy);
            phones = strategy.Apply(phones, keyword).ToList();
        }

        // Sử dụng Iterator Pattern để duyệt danh sách (yêu cầu của thầy)
        var collection = new PhoneCollection(phones);
        var iterator = collection.CreateIterator();
        var result = new List<PhoneDto>();

        while (iterator.HasNext())
        {
            result.Add(Map(iterator.Next()));
        }

        return result;
    }

    public async Task<PhoneDto> CreateAsync(CreatePhoneDto dto, CancellationToken cancellationToken = default)
    {
        var phone = new Phone
        {
            Model = dto.Model,
            Brand = dto.Brand,
            Specifications = dto.Specifications
        };

        var created = await _phoneRepository.AddAsync(phone, cancellationToken);
        return Map(created);
    }

    public async Task<PhoneDto> UpdateAsync(int id, UpdatePhoneDto dto, CancellationToken cancellationToken = default)
    {
        var phone = await _phoneRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Phone not found.");

        phone.Model = dto.Model;
        phone.Brand = dto.Brand;
        phone.Specifications = dto.Specifications;

        var updated = await _phoneRepository.UpdateAsync(phone, cancellationToken);
        return Map(updated);
    }

    public async Task<PhoneDto> UpdateStatusAsync(int id, string action, CancellationToken cancellationToken = default)
    {
        // State Pattern: xử lý chuyển trạng thái (Pending -> Approved/Rejected)
        var phone = await _phoneRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Phone not found.");

        var context = new PhoneReviewStateContext(phone.Status);
        context.ApplyAction(phone, action);

        var updated = await _phoneRepository.UpdateAsync(phone, cancellationToken);
        return Map(updated);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _phoneRepository.DeleteAsync(id, cancellationToken);
    }

    private static PhoneDto Map(Phone phone)
    {
        return new PhoneDto
        {
            Id = phone.Id,
            Model = phone.Model,
            Brand = phone.Brand,
            Specifications = phone.Specifications,
            Status = phone.Status
        };
    }
}
