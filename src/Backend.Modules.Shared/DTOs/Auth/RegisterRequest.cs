using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Auth;

public record RegisterRequest(
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
    string Username,
    
    [Required]
    [EmailAddress]
    string Email,
    
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    string Password
);