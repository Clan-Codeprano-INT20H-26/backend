using Backend.Modules.Order.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Order;

public static class OrderMigrationExtensions
{
    public static void ApplyOrderMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using OrderDbContext dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        
        dbContext.Database.Migrate();
    }
}