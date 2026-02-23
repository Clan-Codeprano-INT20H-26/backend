namespace Backend.Modules.Shared.DTOs.Auth;

public record LoginRequest(
    string email,
    string password
);