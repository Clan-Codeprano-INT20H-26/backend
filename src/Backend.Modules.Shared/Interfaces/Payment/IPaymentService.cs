using Backend.Modules.Shared.DTOs.Payment;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Payment;

public interface IPaymentService
{
    Task<Result<PaymentIntentResponse>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request, Guid userId);
}