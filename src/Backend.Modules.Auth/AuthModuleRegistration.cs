using System.Security.Claims;
using System.Text;
using Backend.Modules.Auth.Application;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Auth.Interfaces.JWT;
using Backend.Modules.Shared.Interfaces.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
namespace Backend.Modules.Auth;

public static class AuthModuleRegistration
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        var secret = config["JWT_SECRET"] ?? "default_secret_key_for_dev_12345"; 
        var issuer = config["JWT_ISSUER"];
        var audience = config["JWT_AUDIENCE"];

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            
            .AddJwtBearer(options =>
            {
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,   
                    ValidateAudience = false, 
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["token"];
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddAuthorization();

        services.AddDbContext<AuthDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));
        
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.AuthController).Assembly);
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.UserController).Assembly);

        return services;
    }
}