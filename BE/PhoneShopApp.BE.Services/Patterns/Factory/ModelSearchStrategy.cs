using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Services.Patterns.Factory;

public class ModelSearchStrategy : IPhoneSearchStrategy
{
    public IEnumerable<Phone> Apply(IEnumerable<Phone> phones, string keyword)
    {
        return phones.Where(x => x.Model.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }
}
