using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Backend.Modules.Auth.Domain;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Shared.DTOs.Auth;
using Backend.Modules.Shared.Interfaces.Auth;
using Backend.Modules.Shared.Interfaces.Image;
using FluentResults; 
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Auth.Application;

public class UserService : IUserService
{
    private readonly AuthDbContext _dbContext;
    private readonly IImageStorage _imageStorage;

    public UserService(AuthDbContext dbContext, IImageStorage imageStorage)
    {
        _dbContext = dbContext;
        _imageStorage = imageStorage;
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
            isAdmin = user.IsAdmin,

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


    public async Task<Result<string>> UpdateAvatarAsync(Guid userId, IFormFile file)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null)
        {
            return Result.Fail("User not found");
        }
        var uploadResult = await _imageStorage.UploadAsync(file);

        if (uploadResult.IsFailed)
        {
            return Result.Fail(uploadResult.Errors);
        }
        
        user.Avatar = uploadResult.Value;
        

        await _dbContext.SaveChangesAsync();

        return Result.Ok(user.Avatar);
    }
}