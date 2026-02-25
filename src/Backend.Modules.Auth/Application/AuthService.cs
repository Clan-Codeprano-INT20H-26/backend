using Backend.Modules.Auth.Domain;
using Backend.Modules.Auth.Infrastructure;
using Backend.Modules.Auth.Interfaces.JWT;
using Backend.Modules.Shared.DTOs.Auth;
using Backend.Modules.Shared.Interfaces.Auth;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Auth.Application;

public class AuthService : IAuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly AuthDbContext _db;

    public AuthService(AuthDbContext database, IJwtTokenService jwtTokenService)
    {
        _db = database;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(string username, string email, string password, CancellationToken ct)
    {
        var userExists = await _db.Users.AsNoTracking()
            .AnyAsync(u => u.Email == email || u.Username == username, ct);

        if (userExists)
            return Result.Fail("User already exists");

        var user = new User
        {
            Email = email.ToLower().Trim(),
            Username = username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsAdmin = false,
            Avatar = null
        };

        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to create user").CausedBy(ex));
        }

        return GenerateAuthResponse(user);
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return Result.Fail("Invalid email or password");
        }

        return GenerateAuthResponse(user);
    }

    public async Task<Result<UserDto>> GetProfileAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return Result.Fail("User not found");

        return Result.Ok(MapToDto(user));
    }
    

    private Result<AuthResponse> GenerateAuthResponse(User user)
    {
        var token = _jwtTokenService.Generate(user.Id, user.Email, user.Username, user.IsAdmin);
        var userDto = MapToDto(user);
        
        return Result.Ok(new AuthResponse(token, userDto));
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            isAdmin = user.IsAdmin,
            avatar = user.Avatar
        };
    }
}