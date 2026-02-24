
using Backend.Modules.Shared.DTOs.Kit;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Kit;

public interface IKitService
{
    
    Task<Result<List<KitResponse>>> GetAllAsync();
    
    Task<Result<KitResponse>> GetByIdAsync(Guid id);
    
    Task<Result<KitResponse>> CreateAsync(CreateKitDto dto);
    
    Task<Result<KitResponse>> UpdateAsync(Guid id, UpdateKitDto dto);
    
    Task<Result> DeleteAsync(Guid id); 
    
    Result<decimal> CalculateTotalPrice(List<KitResponse> kits);
    Task<Result<decimal>> CalculateTotalPriceAsync(IEnumerable<Guid> kitIds);
}