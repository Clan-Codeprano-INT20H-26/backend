using System.ComponentModel.DataAnnotations;
using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Shared.DTOs.Payment;

public record CreatePaymentIntentRequest(
    [Required]
    [MinLength(1, ErrorMessage = "Must contain at least one item")]
    List<OrderItemDto> Items
);