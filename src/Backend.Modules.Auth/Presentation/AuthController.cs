using System.Security.Claims;
using Backend.Modules.Shared.DTOs.Auth;
using Backend.Modules.Shared.Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Auth.Presentation;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("auth/register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(
            username: request.username, 
            email: request.email, 
            password: request.password, 
            ct);

        if (result.IsFailed)
        {
            return BadRequest(new { message = result.Errors.First().Message });
        }
        
        return Ok(result.Value);
    }

    [HttpPost("auth/login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(request.email, request.password, ct);

        if (result.IsFailed)
        {
            return BadRequest(new { message = result.Errors.First().Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("profile")]
    [Authorize] 
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var userIdString = User.FindFirst("id")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new { message = "Invalid token claims" });
        }

        var result = await _authService.GetProfileAsync(userId, ct);

        if (result.IsFailed)
        {
            return NotFound(new { message = result.Errors.First().Message });
        }

        return Ok(result.Value);
    }
}