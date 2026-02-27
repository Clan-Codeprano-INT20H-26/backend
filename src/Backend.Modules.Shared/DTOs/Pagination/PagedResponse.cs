namespace Backend.Modules.Shared.DTOs.Pagination;

public record PagedResponse<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber = 1,
    int PageSize = 5
)
{
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}