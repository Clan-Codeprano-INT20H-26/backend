using Backend.Modules.Order.Application;
using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Shared.Interfaces.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Backend.Modules.Order;
using Microsoft.Extensions.Logging;

using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Infrastructure.Csv;

namespace Backend.Modules.Order;

public static class OrdersModulesRegistration
{
    public static IServiceCollection AddOrdersModules(this IServiceCollection services, IConfiguration manager)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddHttpClient();
        services.AddDbContext<Infrastructure.OrderDbContext>(options =>
            options.UseNpgsql(manager.GetConnectionString("Postgres")));
        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.OrderController).Assembly); //add controllers
        
        services.AddScoped<ICsvParserService, CsvParserService>();
        
        services.AddScoped<IOrderBulkRepository, OrderBulkRepository>();
        
        services.AddScoped<IOrderImportService, OrderImportService>();
        
        return services;
    }
}