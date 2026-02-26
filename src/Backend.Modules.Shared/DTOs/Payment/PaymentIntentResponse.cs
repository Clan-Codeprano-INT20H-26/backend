namespace Backend.Modules.Shared.DTOs.Payment;

public record PaymentIntentResponse(
    string ClientSecret,
    decimal TotalAmount
);