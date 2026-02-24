using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.Modules.Shared.DTOs.Kit;

public class CreateKitDto
{
    [Required]
    public string name { get; set; }
    
    public string description { get; set; }
    
    [Required]
    public string seller { get; set; }

    [Range(0, double.MaxValue)]
    public decimal price { get; set; }
    
    public List<IFormFile> images { get; set; }
}