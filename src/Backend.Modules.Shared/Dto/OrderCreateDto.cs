namespace Backend.Modules.Shared.Dto;

public class OrderCreateDto
{
    public List<Guid> kitId { get; set; } = new();
    public string? latitude { get; set; }
    public string? longitude { get; set; }
}