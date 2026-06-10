using PhoneShopApp.BE.Services.Implementations;
using PhoneShopApp.BE.Services.Interfaces;
using PhoneShopApp.BE.Services.Patterns.Facade;
using PhoneShopApp.BE.Services.Patterns.Factory;
using Microsoft.Extensions.DependencyInjection;

namespace PhoneShopApp.BE.Services.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPhoneService, PhoneService>();
        services.AddScoped<IPhoneShopFacade, PhoneShopFacade>();

        services.AddSingleton<PhoneSearchFactoryMethod>();

        return services;
    }
}
