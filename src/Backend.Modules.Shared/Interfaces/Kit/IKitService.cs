
using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Pagination;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Kit;

public interface IKitService
{
    
    Task<Result<PagedResponse<KitResponse>>> GetAllAsync(KitFilterRequest filter);
    Task<Result<KitResponse>> GetByIdAsync(Guid id);
    Task<Result<List<KitResponse>>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<Result<KitResponse>> CreateAsync(CreateKitRequest request);
    
    Task<Result<KitResponse>> UpdateAsync(Guid id, UpdateKitRequest request);
    
    Task<Result> DeleteAsync(Guid id); 
    
    Result<decimal> CalculateTotalPrice(List<KitResponse> kits);
    public Task<Result<decimal>> CalculateTotalPriceAsync(IEnumerable<CreateOrderItemRequest> kitPackDtos);
}