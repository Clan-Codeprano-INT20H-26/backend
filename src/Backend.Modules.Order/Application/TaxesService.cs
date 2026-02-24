using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Mappers; // Для маппинга
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using FluentResults;

namespace Backend.Modules.Order.Application;

public class TaxesService : ITaxesService
{
    private readonly ITaxHelper _taxHelper;

    public TaxesService(ITaxHelper taxHelper)
    {
        _taxHelper = taxHelper;
    }

    public async Task<Result<TaxesBreakdownDto>> CalculateTaxAsync(string latitude, string longitude)
    {
        if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
            return Result.Fail("Coordinates are required");

        try 
        {
            var taxes = await _taxHelper.GetTaxesBreakdownAsync(latitude, longitude);
            
            var dto = new TaxesBreakdownDto(
                taxes.StateRate,
                taxes.CountryRate,
                taxes.CityRate,
                taxes.SpecialRates,
                taxes.Jurisdictions
            );

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to calculate tax").CausedBy(ex));
        }
    }
}