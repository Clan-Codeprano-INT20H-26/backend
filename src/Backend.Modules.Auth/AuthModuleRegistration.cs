using Backend.Modules.Auth.Application;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Auth.Interfaces.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Auth;

public static class AuthModuleRegistration
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(opt =>
            opt.UseNpgsql(config["ConnectionStrings__Postgres"]));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<AuthService>();

        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.AuthController).Assembly);

        return services;
    }
}