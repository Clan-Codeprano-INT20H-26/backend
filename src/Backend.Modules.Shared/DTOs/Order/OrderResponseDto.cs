namespace Backend.Modules.Shared.DTOs.Order;

public record OrderResponseDto(
    Guid Id,                    
    Guid UserId,
    List<Guid> KitIds,          
    decimal SubTotal,
    string Status,             
    string Latitude,
    string Longitude,
    TaxesBreakdownDto? Taxes,   
    decimal TotalAmount
);