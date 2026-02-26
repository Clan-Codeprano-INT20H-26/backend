using Backend.Module.Tax.Domain;
using Backend.Module.Tax.Infrastructure;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Tax;
using Backend.Modules.Shared.Interfaces.Tax;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using FluentResults;

namespace Backend.Module.Tax.Application;

public class TaxService : ITaxService
{
    private readonly TaxDbContext _context;

    public TaxService(TaxDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TaxBreakdownResponse>> CalculateTaxesAsync(decimal lat, decimal lon)
    {
        try 
        {
            var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var location = factory.CreatePoint(new Coordinate((double)lon, (double)lat));
            
            var county = await _context.Counties.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Geometry.Contains(location));

            if (county == null)
            {
                return Result.Fail("Location is outside of supported NY counties");
            }

            var city = await _context.Cities.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Geometry.Contains(location));

            var stateRate = 0.04m;

            var countyRateObj = await _context.TaxRates
                .FirstOrDefaultAsync(r => r.Type == "County" && county.Name.Contains(r.JurisdictionName));
            var countyRate = countyRateObj?.Rate ?? 0m;

            decimal cityRate = 0m;
            if (city != null)
            {
                var cityRateObj = await _context.TaxRates
                    .FirstOrDefaultAsync(r => r.Type == "City" && city.Name.Contains(r.JurisdictionName));
                cityRate = cityRateObj?.Rate ?? 0m;
            }

            decimal effectiveCountyRate = countyRate;
            if (cityRate > 0)
            {
                effectiveCountyRate = 0; 
            }
            //transport tax
            var mctdCounties = new[] { "Bronx", "Kings", "New York", "Queens", "Richmond", "Dutchess", "Nassau", "Orange", "Putnam", "Rockland", "Suffolk", "Westchester" };
            decimal specialRate = 0m;
            if (mctdCounties.Any(m => county.Name.Contains(m, StringComparison.OrdinalIgnoreCase)))
            {
                specialRate = 0.00375m;
            }

            var jurisdictions = new List<string> { "New York State", county.Name };
            if (city != null) jurisdictions.Add(city.Name);
            if (specialRate > 0) jurisdictions.Add("MCTD District");

            return Result.Ok(new TaxBreakdownResponse(
                StateRate: stateRate,
                CountyRate: effectiveCountyRate,
                CityRate: cityRate,
                SpecialRates: specialRate,
                Jurisdictions: jurisdictions
            ));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Tax calculation failed").CausedBy(ex));
        }
    }
}