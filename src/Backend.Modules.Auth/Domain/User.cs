namespace Backend.Modules.Auth.Domain;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Avatar { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// User {
// id: UUID,
// username: string,
// email: string,
// avatar: string,
// isAdmin: boolean
// }