using Backend.Modules.Order.Domain;

namespace Backend.Modules.Order.Application.Interfaces;

public interface ITaxHelper
{
    public Task<TaxesBreakdown> GetTaxesBreakdownAsync(string latitude, string longitude);
}