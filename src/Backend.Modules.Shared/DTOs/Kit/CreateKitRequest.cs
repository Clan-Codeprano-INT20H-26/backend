using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.Modules.Shared.DTOs.Kit;

public record CreateKitRequest(
    [Required]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    string Name,

    [Required]
    [MaxLength(1024)]
    string Description,
    
    [Required]
    string Seller,

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    decimal Price,
    
    [Required]
    List<IFormFile> Images
);