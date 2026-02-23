using Backend.Modules.Auth.Interfaces.JWT;  
using Backend.Modules.Auth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Backend.Modules.Auth.Domain;


namespace Backend.Modules.Auth.Application;
public class AuthService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly AuthDbContext _db; 
    
    
    public AuthService(AuthDbContext database, IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
        _db = database;
    }

    public async Task<string> RegisterAsync(string username, string email, string password, 
                    bool isAdmin, CancellationToken ct)
    {
        // проверка 
        if (await _db.Users.AnyAsync(u => u.Email == email || u.Username == username, ct))
            throw new InvalidOperationException("User with this email or username already exists");

        var user = new User
        {
            Email = email.ToLower().Trim(),
            Username = username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsAdmin = isAdmin, Avatar = null
        }; 
        
        // сохранение
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
        
        var token = _jwtTokenService.Generate(user.Id, user.Email, user.Username, user.IsAdmin);
        
        return token;
    }
    
    public async Task<string> LoginAsync(string email, string password, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user == null)
            throw new InvalidOperationException("User with this email not found");
        
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new InvalidOperationException("Password is incorrect");
        
        var token = _jwtTokenService.Generate(user.Id, user.Email, user.Username, user.IsAdmin);
        
        return token;
    }
    
}