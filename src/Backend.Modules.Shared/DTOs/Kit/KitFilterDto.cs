namespace Backend.Modules.Shared.DTOs.Kit;

public class KitFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    
    public string? SearchTerm { get; set; } 
    public string? Seller { get; set; }     
    public decimal? MinPrice { get; set; }  
    public decimal? MaxPrice { get; set; }  
    
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = true;
}