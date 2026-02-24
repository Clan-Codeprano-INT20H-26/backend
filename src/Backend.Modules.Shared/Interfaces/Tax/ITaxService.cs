using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Tax;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Tax;

public interface ITaxService
{
    public Task<Result<TaxesBreakdownDto>> CalculateTaxesAsync(decimal lat, decimal lon);
}