using Backend.Modules.Order.Application;
using Backend.Modules.Shared.Dto;
using Microsoft.AspNetCore.Mvc;
namespace Backend.Modules.Order.Presentation;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpPost]
    public async Task<IActionResult> Create(OrderCreateDto dto)
    {
        var userId = Guid.NewGuid(); //GetFromJwt
        
        var result = await _orderService.CreateOrderAsync(dto, userId);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.First().Message);
        }

        return Ok(result.Value);
    }
}