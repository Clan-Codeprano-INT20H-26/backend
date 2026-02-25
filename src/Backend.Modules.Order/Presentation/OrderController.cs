using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Pagination;
using Backend.Modules.Shared.Interfaces.Auth;
using Backend.Modules.Shared.Interfaces.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Order.Presentation;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public OrderController(IOrderService orderService, IUserService userService)
    {
        _orderService = orderService;
        _userService = userService;
    }
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
    {
        var userId = _userService.GetUserIdFromJwt(User);
        if (userId is null) return Unauthorized();

        var result = await _orderService.CreateOrderAsync(dto, userId.Value);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(result.Value);
    }
    [Authorize]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id)
    {
        var userId = _userService.GetUserIdFromJwt(User);
        if (userId is null) return Unauthorized();
        var result = await _orderService.GetByIdAsync(id, userId.Value);

        if (result.IsFailed)
        {
            return NotFound(new { message = result.Errors.First().Message });
        }

        return Ok(result.Value);
    }
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<IActionResult> GetAll([FromQuery] OrderFilterDto filter)
    {
        var userId = _userService.GetUserIdFromJwt(User);
        if (userId is null) return Unauthorized();
        
        var result = await _orderService.GetAllAsync(userId.Value, filter);
        
        if (result.IsFailed)
        {
            return BadRequest(new { message = result.Errors.First().Message });
        }
        
        return Ok(result.Value);
    }
}