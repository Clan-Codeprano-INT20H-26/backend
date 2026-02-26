using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Order.Application.Mappers;

public static class OrderItemMapper
{

    public static OrderItemDto ToDto(this OrderItem entity)
    {
        if (entity == null) return null;

        return new OrderItemDto
        {
            kitId = entity.KitId,
            count = entity.Quantity
        };
    }

    public static OrderItem ToDomain(this OrderItemDto dto)
    {
        if (dto == null) return null;

        return new OrderItem
        {
            KitId = dto.kitId,
            Quantity = dto.count
        };
    }



    public static List<OrderItemDto> ToDtos(this IEnumerable<OrderItem> entities)
    {

        return entities?.Select(e => e.ToDto()).ToList() ?? new List<OrderItemDto>();
    }

    public static List<OrderItem> ToDomains(this IEnumerable<OrderItemDto> dtos)
    {
        return dtos?.Select(d => d.ToDomain()).ToList() ?? new List<OrderItem>();
    }
}