namespace Backend.Modules.Shared.DTOs.Order;

public class OrderFilterRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    
    public string? SortBy { get; set; } // "Date", "Price"
    public bool IsDescending { get; set; } = true;
}