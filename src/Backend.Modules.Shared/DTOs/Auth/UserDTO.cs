namespace Backend.Modules.Shared.DTOs.Auth;

public class UserDto
{
    public Guid id { get; set; }
    public string username { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public bool isAdmin { get; set; }
    public string? avatar { get; set; } = string.Empty;
}