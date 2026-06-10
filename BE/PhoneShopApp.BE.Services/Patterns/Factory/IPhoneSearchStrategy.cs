using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Services.Patterns.Factory;

public interface IPhoneSearchStrategy
{
    IEnumerable<Phone> Apply(IEnumerable<Phone> phones, string keyword);
}
