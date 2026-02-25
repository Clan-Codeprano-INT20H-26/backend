using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Order.Application.Mappers;

public static class KitPackMapper
{

    public static KitPackDto ToDto(this KitPack entity)
    {
        if (entity == null) return null;

        return new KitPackDto
        {
            kitId = entity.KitId,
            count = entity.Count
        };
    }

    public static KitPack ToDomain(this KitPackDto dto)
    {
        if (dto == null) return null;

        return new KitPack
        {
            KitId = dto.kitId,
            Count = dto.count
        };
    }



    public static List<KitPackDto> ToDtos(this IEnumerable<KitPack> entities)
    {

        return entities?.Select(e => e.ToDto()).ToList() ?? new List<KitPackDto>();
    }

    public static List<KitPack> ToDomains(this IEnumerable<KitPackDto> dtos)
    {
        return dtos?.Select(d => d.ToDomain()).ToList() ?? new List<KitPack>();
    }
}