namespace Backend.Modules.Shared.DTOs.Kit;

public class KitFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    public string? SearchTerm { get; set; } 
    public string? Seller { get; set; }     
    public decimal? MinPrice { get; set; }  
    public decimal? MaxPrice { get; set; }  
}