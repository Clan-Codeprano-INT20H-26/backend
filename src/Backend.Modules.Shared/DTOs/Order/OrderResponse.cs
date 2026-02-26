using Backend.Modules.Shared.DTOs.Tax;

namespace Backend.Modules.Shared.DTOs.Order;

public record OrderResponse(
    Guid Id,                    
    Guid UserId,
    List<OrderItemDto> Items,
    decimal SubTotal,
    string Status,             
    string Latitude,
    string Longitude,
    decimal TaxAmount,
    decimal CompositeTaxRate,
    decimal TotalAmount,
    DateTime CreatedAt,
    TaxBreakdownResponse? Taxes
);
