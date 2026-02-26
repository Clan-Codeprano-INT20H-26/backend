namespace Backend.Modules.Shared.DTOs.Kit;

public record KitFilterRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? Seller = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    string? SortBy = null,
    bool IsDescending = true
);