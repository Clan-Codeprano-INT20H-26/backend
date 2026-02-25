using Backend.Modules.Shared.Infrastructure;
using Backend.Modules.Shared.Interfaces.Image;
using Backend.Modules.Shared.Interfaces.Kit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Shared;

public static class SharedModuleRegistration
{
    public static IServiceCollection AddSharedModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<IImageStorage, ImageStorage>();
        return services;
    }
}