using Backend.Modules.Shared.Interfaces.Auth;

namespace Backend.Modules.Shared.DTOs.Auth;

public record AuthResponse(string Token, UserDto User);