using FluentResults;
using Microsoft.AspNetCore.Http;
using Backend.Modules.Shared.DTOs.Auth;
using System.Security.Claims;

namespace Backend.Modules.Shared.Interfaces.Auth;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Guid? GetUserIdFromJwt(ClaimsPrincipal principal);
    
    Task<Result<string>> UpdateAvatarAsync(Guid userId, IFormFile file);
}