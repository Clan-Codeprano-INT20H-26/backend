using Backend.Modules.Shared.DTOs.Kit;

namespace Backend.Module.Kit.Application.Mapper;

public static class KitMapper
{
    public static KitResponse ToResponse(this Domain.Kit kit)
    {
        if (kit == null)
            return null;

        return new KitResponse(
            kit.Id,
            kit.Name,
            kit.Description,
            kit.Seller,
            kit.Price,
            kit.Images != null ? new List<string>(kit.Images) : new List<string>());
    }
    
    public static List<KitResponse> ToResponseList(this IEnumerable<Domain.Kit> kits)
    {
        return kits.Select(ToResponse).ToList();
    }
}