namespace PhoneShopApp.BE.Services.Patterns.Factory;

// Factory Method Pattern: tạo strategy tìm kiếm phù hợp theo searchBy.
public class PhoneSearchFactoryMethod
{
    public virtual IPhoneSearchStrategy Create(string? searchBy)
    {
        return searchBy?.ToLowerInvariant() switch
        {
            "model" => new ModelSearchStrategy(),
            "brand" => new BrandSearchStrategy(),
            _ => new AllFieldSearchStrategy()
        };
    }
}
