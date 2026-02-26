using Backend.Modules.Shared.DTOs.Payment;
using Backend.Modules.Shared.Interfaces.Kit;
using Backend.Modules.Shared.Interfaces.Payment;
using FluentResults;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Backend.Modules.Payment.Application;

public class PaymentService : IPaymentService
{
    private readonly IKitService _kitService;

    public PaymentService(IKitService kitService, IConfiguration configuration)
    {
        _kitService = kitService;
        StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe:SecretKey is not configured");
    }

    public async Task<Result<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, Guid userId)
    {
        var kitResult = await _kitService.CalculateTotalPriceAsync(request.Items);
        if (kitResult.IsFailed)
            return Result.Fail(kitResult.Errors);

        var totalAmount = kitResult.Value;
        var amountInCents = (long)Math.Round(totalAmount * 100, 0, MidpointRounding.AwayFromZero);

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId.ToString() }
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return Result.Ok(new PaymentIntentResponse(
                paymentIntent.ClientSecret,
                totalAmount
            ));
        }
        catch (StripeException ex)
        {
            return Result.Fail(new Error($"Stripe error: {ex.StripeError?.Message ?? ex.Message}"));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to create payment intent").CausedBy(ex));
        }
    }
}