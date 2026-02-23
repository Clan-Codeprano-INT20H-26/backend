using Backend.Modules.Auth.Application;
using Backend.Modules.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Http; // Обязательно добавь для StatusCodes
using Microsoft.AspNetCore.Mvc;

namespace Backend.Modules.Auth.Presentation;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var token = await _authService.RegisterAsync(
            request.username, 
            request.email, 
            request.password, 
            request.isAdmin, 
            ct);
    
        return Ok(new AuthResponse(token));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request.email, request.password, ct);
        return Ok(new AuthResponse(token));
    }
}