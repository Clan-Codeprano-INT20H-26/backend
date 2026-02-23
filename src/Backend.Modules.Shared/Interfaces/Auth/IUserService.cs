using System.Security.Claims;
using Backend.Modules.Shared.DTOs.Auth;

namespace Backend.Modules.Shared.Interfaces.Auth;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Guid? GetUserIdFromJwt(ClaimsPrincipal principal);
}