using PhoneShopApp.BE.Core.Interfaces.Repositories;
using PhoneShopApp.BE.Infrastructure.Data;
using PhoneShopApp.BE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PhoneShopApp.BE.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' was not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));

        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IPhoneRepository, PhoneRepository>();

        return services;
    }
}
