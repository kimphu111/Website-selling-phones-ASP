using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Services.Patterns.Factory;

public class BrandSearchStrategy : IPhoneSearchStrategy
{
    public IEnumerable<Phone> Apply(IEnumerable<Phone> phones, string keyword)
    {
        return phones.Where(x => x.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}
