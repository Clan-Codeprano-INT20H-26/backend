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
    private readonly IOrderImportService _orderImportService;

    public OrderController(
        IOrderService orderService, 
        IUserService userService, 
        IOrderImportService orderImportService)
    {
        _orderService = orderService;
        _userService = userService;
        _orderImportService = orderImportService;
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

    [Authorize]
    [HttpPost("import")]
    [RequestSizeLimit(long.MaxValue)] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ImportCsv(IFormFile file, CancellationToken ct)
    {
        var userId = _userService.GetUserIdFromJwt(User);
        if (userId is null) return Unauthorized();

        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        await using var stream = file.OpenReadStream();

        var result = await _orderImportService.ImportOrdersAsync(userId.Value, stream, ct);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(e => e.Message));
        }

        return Ok(new { message = "CSV successfully imported" });
    }
}