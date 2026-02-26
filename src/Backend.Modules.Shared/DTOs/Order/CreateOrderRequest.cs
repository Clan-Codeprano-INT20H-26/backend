using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Order;

public class CreateOrderRequest
{
    public List<OrderItemDto> kitPacks { get; set; } = new();
    [Required]
    public string? latitude { get; set; }
    [Required]
    public string? longitude { get; set; }
}