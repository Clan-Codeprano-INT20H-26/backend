using Backend.Modules.Shared.DTOs.Tax;

namespace Backend.Modules.Shared.DTOs.Order;

public record OrderResponseDto(
    Guid Id,                    
    Guid UserId,
    List<KitPackDto> KitPacks,
    decimal SubTotal,
    string Status,             
    string Latitude,
    string Longitude,
    decimal TaxAmount ,
    decimal CompositeTaxRate ,
    decimal TotalAmount ,
    TaxesBreakdownDto? Taxes
);