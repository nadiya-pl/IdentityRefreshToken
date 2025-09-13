using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProtectedAPI.Models;
using System.Text;
using System.Text.Json;

namespace ProtectedAPI.Extensions;

public static class AuthServiceExtension
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JWT:SecretKey"];
        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero
               };

               options.Events = new JwtBearerEvents
               {
                   OnChallenge = context =>
                   {
                       context.HandleResponse(); // отключаем стандартное поведение (401 без тела)
                       context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                       context.Response.ContentType = "application/json";

                       var response = new ProtectedResponse
                       {
                           Success = false,
                           Message = "Unauthorized: Token is missing or invalid"
                       };

                       return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                   },

                   OnForbidden = context =>
                   {
                       context.Response.StatusCode = StatusCodes.Status403Forbidden;
                       context.Response.ContentType = "application/json";

                       var response = new ProtectedResponse
                       {
                           Success = false,
                           Message = "Forbidden: You do not have access to this resource"
                       };

                       return context.Response.WriteAsync(JsonSerializer.Serialize(response));
                   }
               };
           });

        return services;
    }
}
