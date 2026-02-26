namespace Backend.Modules.Shared.DTOs.Kit;

public record KitResponse(
    Guid Id,
    string Name,
    string Description,
    string Seller,
    decimal Price,
    List<string> Images
);