using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Order;

public record CreateOrderItemRequest(
    [Required]
    Guid KitId,
    
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    int Quantity
);