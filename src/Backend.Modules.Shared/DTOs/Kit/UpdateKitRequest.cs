using Microsoft.AspNetCore.Http;

namespace Backend.Modules.Shared.DTOs.Kit;

public class UpdateKitRequest
{
    public string? name { get; set; }
    public string? description { get; set; }
    public decimal? price { get; set; }
    public List<IFormFile>? newImages { get; set; }
}