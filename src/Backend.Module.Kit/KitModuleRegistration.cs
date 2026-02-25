using Backend.Module.Kit.Application;
using Backend.Module.Kit.Infrastructure;
using Backend.Module.Kit.Presentation;
using Backend.Modules.Shared.Interfaces.Kit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Module.Kit;

public static class KitModuleRegistration
{
    public static IServiceCollection AddKitModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<KitDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        services.AddScoped<IKitService, KitService>();
        services.AddScoped<KitSeeder>();
        services.AddControllers()
            .AddApplicationPart(typeof(KitController).Assembly);
        return services;
    }
}