using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Shared.DTOs.Auth;

public record LoginRequest(
    [Required]
    [EmailAddress]
    string Email,
    
    [Required]
    string Password
);