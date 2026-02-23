namespace Backend.Modules.Auth.Interfaces.JWT;


// public string Generate(Guid userId, string email, string username, bool isAdmin) - class JwtTokenService

public interface IJwtTokenService
{
    string Generate(Guid userId, string email, string username, bool isAdmin);
}