namespace Backend.Modules.Shared.DTOs.Auth;

public record UserResponse(
    Guid Id, 
    string Username, 
    string Email, 
    bool IsAdmin, 
    string? Avatar
);