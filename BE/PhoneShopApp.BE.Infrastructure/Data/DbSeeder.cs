using PhoneShopApp.BE.Core.Entities;

namespace PhoneShopApp.BE.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!dbContext.Users.Any())
        {
            dbContext.Users.Add(new AppUser
            {
                Username = "admin",
                FullName = "System Admin",
                PasswordHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9",
                Role = "Admin"
            });
        }

        if (!dbContext.Phones.Any())
        {
            dbContext.Phones.AddRange(
                new Phone { Model = "iPhone 15 Pro Max", Brand = "Apple", Specifications = "A17 Pro chip, 8GB RAM, 256GB, Titanium" },
                new Phone { Model = "Galaxy S24 Ultra", Brand = "Samsung", Specifications = "Snapdragon 8 Gen 3, 12GB RAM, 512GB, Titanium Gray" },
                new Phone { Model = "Pixel 8 Pro", Brand = "Google", Specifications = "Google Tensor G3, 12GB RAM, 256GB, AI features" }
            );
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
