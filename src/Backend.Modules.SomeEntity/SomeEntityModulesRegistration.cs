using Backend.Modules.Shared.Interfaces.SomeEntity;
using Backend.Modules.SomeEntity.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.SomeEntity;

public static class SomeEntityModulesRegistration
{
    public static IServiceCollection AddEntityModules(this IServiceCollection services, IConfiguration manager)
    {
        services.AddScoped<ISomeEntityService, SomeEntityService>();
        // services.AddDbContext<Context>(options =>
        //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.SomeEntityController).Assembly); //add controllers
        return services;
    }
}