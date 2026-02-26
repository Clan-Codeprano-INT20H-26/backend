using Backend.Modules.Payment.Application;
using Backend.Modules.Shared.Interfaces.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Payment;

public static class PaymentModuleRegistration
{
    public static IServiceCollection AddPaymentModule(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddControllers()
            .AddApplicationPart(typeof(Presentation.PaymentController).Assembly);

        return services;
    }
}