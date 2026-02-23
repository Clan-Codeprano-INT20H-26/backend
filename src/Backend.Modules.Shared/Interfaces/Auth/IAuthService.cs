namespace Backend.Modules.Shared.Interfaces.Auth;

public interface IAuthService 
{
// public async Task<string> LoginAsync(string email, string password, CancellationToken ct
    Task<string> LoginAsync(string email, string password, CancellationToken ct);
    Task<string> RegisterAsync(string username, string email, string password, 
        bool isAdmin, CancellationToken ct);
    
}