using Backend.Modules.Order.Application;
using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Shared.Interfaces.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Order;

public static class OrdersModulesRegistration
{
    public static IServiceCollection AddOrdersModules(this IServiceCollection services, IConfiguration manager)
    {
        services.AddScoped<ITaxHelper, TaxHelper>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddHttpClient();
        services.AddDbContext<Infrastructure.OrderDbContext>(options =>
            options.UseNpgsql(manager.GetConnectionString("Postgres")));
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.OrderController).Assembly); //add controllers
        return services;
    }
}