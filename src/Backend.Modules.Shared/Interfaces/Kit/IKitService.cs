
using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.DTOs.Pagination;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Kit;

public interface IKitService
{
    
    Task<Result<PagedResult<KitResponse>>> GetAllAsync(KitFilterDto filter);
    Task<Result<KitResponse>> GetByIdAsync(Guid id);
    
    Task<Result<KitResponse>> CreateAsync(CreateKitDto dto);
    
    Task<Result<KitResponse>> UpdateAsync(Guid id, UpdateKitDto dto);
    
    Task<Result> DeleteAsync(Guid id); 
    
    Result<decimal> CalculateTotalPrice(List<KitResponse> kits);
    Task<Result<decimal>> CalculateTotalPriceAsync(IEnumerable<Guid> kitIds);
}