using Backend.Modules.Order.Application;
using Backend.Modules.Shared.DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Order.Presentation;

[ApiController]
[Route("[controller]")] 
public class TaxController : ControllerBase
{
    private readonly TaxesService _taxesService;

    public TaxController(TaxesService taxesService)
    {
        _taxesService = taxesService;
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(TaxesBreakdownDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaxesBreakdownDto>> CalculateTax(
        [FromQuery] string latitude, 
        [FromQuery] string longitude)
    {
        var result = await _taxesService.CalculateTaxAsync(latitude, longitude);

        if (result.IsFailed)
        {
            return BadRequest(new { Errors = result.Errors.Select(e => e.Message) });
        }
        return Ok(result.Value);
    }
}