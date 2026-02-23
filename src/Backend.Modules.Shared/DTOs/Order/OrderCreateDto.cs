using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Order;

public class OrderCreateDto
{
    public List<Guid> kitId { get; set; } = new();
    [Required]
    public string? latitude { get; set; }
    [Required]
    public string? longitude { get; set; }
}