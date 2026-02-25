using Backend.Modules.Shared.DTOs.Auth;
using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Auth;

public interface IAuthService 
{
    Task<Result<AuthResponse>> RegisterAsync(string username, string email, string password, CancellationToken ct);
    
    Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct);
    
    Task<Result<UserDto>> GetProfileAsync(Guid userId, CancellationToken ct);
    
}