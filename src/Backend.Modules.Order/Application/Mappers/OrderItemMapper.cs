using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Order.Application.Mappers;

public static class OrderItemMapper
{
    public static OrderItem ToDomain(this CreateOrderItemRequest dto)
    {
        if (dto == null) return null!;

        return new OrderItem
        {
            KitId = dto.KitId,
            Quantity = dto.Quantity,
        };
    }

    public static List<OrderItem> ToDomains(this IEnumerable<CreateOrderItemRequest> dtos)
    {
        return dtos?.Select(d => d.ToDomain()).ToList() ?? new List<OrderItem>();
    }
}