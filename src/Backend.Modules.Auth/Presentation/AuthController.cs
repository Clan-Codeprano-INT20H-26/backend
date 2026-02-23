using Backend.Modules.Auth.Application;
using Backend.Modules.Shared.DTOs.Auth;
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
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _authService.LoginAsync(request.email, request.password, ct);
        return Ok(new AuthResponse(token));
    }
}