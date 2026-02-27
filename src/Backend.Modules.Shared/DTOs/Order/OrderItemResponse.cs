using Backend.Modules.Shared.DTOs.Kit;

namespace Backend.Modules.Shared.DTOs.Order;

public record OrderItemResponse(
    KitResponse Kit, 
    int Quantity
    );