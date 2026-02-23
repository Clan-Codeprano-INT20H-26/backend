using Backend.Modules.Auth.Application;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Auth.Interfaces.JWT;
using Backend.Modules.Shared.Interfaces.Auth;
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
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.AuthController).Assembly);
        return services;
    }
}