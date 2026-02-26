using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.Modules.Shared.DTOs.Kit;

public record UpdateKitRequest(
    [MinLength(3)]
    string? Name,
    
    [MaxLength(1024)]
    string? Description,
    
    [Range(0.01, double.MaxValue)]
    decimal? Price,
    
    List<IFormFile>? NewImages
);