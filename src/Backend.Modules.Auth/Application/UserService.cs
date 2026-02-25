using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Shared.DTOs.Auth;
using Backend.Modules.Shared.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Auth.Application;

public class UserService : IUserService
{
    private readonly AuthDbContext _dbContext;

    public UserService(AuthDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
            return null;

        return new UserDto
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            isAdmin = user.IsAdmin
        };
    }
    
    
    public Guid? GetUserIdFromJwt(ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(sub, out var userId))
            return userId;

        return null;
    }
}