using Backend.Module.Tax.Application;
using Backend.Module.Tax.Infrastructure;
using Backend.Modules.Shared.Interfaces.Tax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Module.Tax;

public static class TaxModulesRegistration
{
    public static IServiceCollection AddTaxModules(this IServiceCollection services, IConfiguration manager)
    {
        services.AddDbContext<TaxDbContext>(options =>
            options.UseNpgsql(
                manager.GetConnectionString("Postgres"), 
                o => o.UseNetTopologySuite()
            ));

        services.AddScoped<ITaxService, TaxService>();
        
        services.AddScoped<DataSeeder>(); 

        return services;
    }
}