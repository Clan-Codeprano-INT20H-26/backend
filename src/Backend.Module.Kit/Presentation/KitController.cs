using Backend.Modules.Shared.DTOs.Kit;
using Backend.Modules.Shared.DTOs.Pagination;
using Backend.Modules.Shared.Interfaces.Kit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Module.Kit.Presentation;

[ApiController]
[Route("[controller]")]
public class KitController : ControllerBase
{
    private readonly IKitService _kitService;

    public KitController(IKitService kitService)
    {
        _kitService = kitService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<KitResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] KitFilterRequest filter)
    {
        var result = await _kitService.GetAllAsync(filter);

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
    public async Task<IActionResult> Create([FromForm] CreateKitRequest request)
    {
        var result = await _kitService.CreateAsync(request);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateKitRequest request)
    {
        var result = await _kitService.UpdateAsync(id, request);

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
    
}