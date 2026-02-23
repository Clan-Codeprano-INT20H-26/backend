using Backend.Modules.Auth.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Modules.Auth;

public static class AuthMigrationExtensions
{
    public static void ApplyAuthMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        
        dbContext.Database.Migrate();
    }
}