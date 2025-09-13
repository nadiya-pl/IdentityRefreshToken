using AuthAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace AuthAPI.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Identity")));

        return services;
    }
}
