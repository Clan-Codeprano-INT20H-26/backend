using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Order;

public record CreateOrderRequest(
    [Required]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    List<OrderItemDto> Items, 

    [Required]
    string Latitude,

    [Required]
    string Longitude
);