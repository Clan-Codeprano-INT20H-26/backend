using Backend.Modules.Shared.DTOs.Payment;
using Backend.Modules.Shared.Interfaces.Auth;
using Backend.Modules.Shared.Interfaces.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Payment.Presentation;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;

    public PaymentController(IPaymentService paymentService, IUserService userService)
    {
        _paymentService = paymentService;
        _userService = userService;
    }

    [HttpPost("create-intent")]
    [ProducesResponseType(typeof(PaymentIntentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateIntent([FromBody] CreatePaymentIntentRequest request)
    {
        var userId = _userService.GetUserIdFromJwt(User);
        if (userId is null) return Unauthorized();

        var result = await _paymentService.CreatePaymentIntentAsync(request, userId.Value);

        if (result.IsFailed)
            return BadRequest(result.Errors.Select(e => e.Message));

        return Ok(result.Value);
    }
}