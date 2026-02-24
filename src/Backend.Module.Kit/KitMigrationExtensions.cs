using Backend.Module.Kit.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Module.Kit;

public static class KitMigrationExtensions
{
    public static void ApplyKitMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using KitDbContext dbContext = scope.ServiceProvider.GetRequiredService<KitDbContext>();
        
        dbContext.Database.Migrate();
    }
}