namespace Backend.Modules.Shared.DTOs.Order;

public record OrderFilterRequest(
    int PageNumber = 1,
    int PageSize = 10,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SortBy = null,
    bool IsDescending = true
);