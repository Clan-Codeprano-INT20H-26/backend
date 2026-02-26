using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Order;

public record UpdateOrderRequest(
    string? Status,
    string? Latitude,
    string? Longitude
);