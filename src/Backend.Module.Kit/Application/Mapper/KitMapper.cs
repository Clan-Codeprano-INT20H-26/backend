using Backend.Modules.Shared.DTOs.Kit;

namespace Backend.Module.Kit.Application.Mapper;

public static class KitMapper
{
    public static KitResponse ToResponse(this Domain.Kit kit)
    {
        if (kit == null)
            return null;

        return new KitResponse
        {
            id = kit.Id,
            name = kit.Name,
            description = kit.Description,
            seller = kit.Seller,
            price = kit.Price,
            images = kit.Images != null ? new List<string>(kit.Images) : new List<string>()
        };
    }
    
    public static List<KitResponse> ToResponseList(this IEnumerable<Domain.Kit> kits)
    {
        return kits.Select(ToResponse).ToList();
    }
}