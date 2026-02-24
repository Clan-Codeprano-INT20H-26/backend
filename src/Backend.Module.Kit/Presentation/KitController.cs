using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.Interfaces.Kit;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Module.Kit.Presentation;

[ApiController]
[Route("api/[controller]")]
public class KitController : ControllerBase
{
    private readonly IKitService _kitService;

    public KitController(IKitService kitService)
    {
        _kitService = kitService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _kitService.GetAllAsync();

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _kitService.GetByIdAsync(id);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateKitDto dto)
    {
        var result = await _kitService.CreateAsync(dto);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateKitDto dto)
    {
        var result = await _kitService.UpdateAsync(id, dto);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _kitService.DeleteAsync(id);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return NoContent();
    }

    [HttpGet("total-price")]
    public async Task<IActionResult> GetTotalPrice()
    {
        var kitsResult = await _kitService.GetAllAsync();

        if (kitsResult.IsFailed)
        {
            return BadRequest(kitsResult.Errors.Select(e => e.Message));
        }

        var calculationResult = _kitService.CalculateTotalPrice(kitsResult.Value);

        if (calculationResult.IsFailed)
        {
            return BadRequest(calculationResult.Errors.Select(e => e.Message));
        }

        return Ok(new { TotalPrice = calculationResult.Value });
    }
}