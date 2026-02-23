namespace Backend.Modules.Shared.DTOs.Auth;

public record RegisterRequest(
    string username,
    string email,
    string password,
    bool isAdmin
);