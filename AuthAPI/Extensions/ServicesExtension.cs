using AuthAPI.Services;
using AuthAPI.TokenService;

namespace AuthAPI.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        return services;
    }
}
