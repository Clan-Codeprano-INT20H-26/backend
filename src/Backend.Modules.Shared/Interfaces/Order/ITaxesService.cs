using Backend.Modules.Shared.DTOs.Order;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface ITaxesService
{
    public Task<Result<TaxesBreakdownDto>> CalculateTaxAsync(string latitude, string longitude);
}