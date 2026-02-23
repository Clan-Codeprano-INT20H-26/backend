namespace Backend.Modules.Shared.DTOs.Order;

public class OrderCreateDto
{
    public List<Guid> kitId { get; set; } = new();
    public string? latitude { get; set; }
    public string? longitude { get; set; }
}