using Backend.Module.Tax.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Module.Tax;

public static class TaxMigrationExtensions
{
    public static void ApplyTaxMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using TaxDbContext dbContext = scope.ServiceProvider.GetRequiredService<TaxDbContext>();
        
        dbContext.Database.Migrate();
    }
}