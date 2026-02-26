using System.Globalization;
using Backend.Modules.Shared.DTOs.Tax;
using Backend.Modules.Shared.Interfaces.Order;
using Backend.Modules.Shared.Interfaces.Tax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Module.Tax.Presentation;

[ApiController]
[Route("[controller]")] 
public class TaxController : ControllerBase
{
    private readonly ITaxService _taxesService;

    public TaxController(ITaxService taxesService)
    {
        _taxesService = taxesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(TaxBreakdownResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<decimal>> CalculateTax(
        [FromQuery] string latitude, 
        [FromQuery] string longitude
        )
    {
        if (!decimal.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var latDecimal))
        {
            return BadRequest(new { Errors = new[] { "Invalid latitude format. Use dot as separator (e.g. 55.1234)" } });
        }
        
        if (!decimal.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lonDecimal))
        {
            return BadRequest(new { Errors = new[] { "Invalid longitude format. Use dot as separator (e.g. 37.1234)" } });
        }
        
        var result = await _taxesService.CalculateTaxesAsync(latDecimal, lonDecimal);
        
        if (result.IsFailed)
        {
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        }

        var value = result.Value;
        var compositeTaxRate = value.CityRate+value.CountyRate+value.SpecialRates+value.StateRate;
        return Ok(compositeTaxRate);
    }
}