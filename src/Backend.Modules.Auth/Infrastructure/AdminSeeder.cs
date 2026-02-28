using System.Text.Json;
using Backend.Modules.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Auth.Infrastructure;

public class AdminSeeder
    {
        private readonly AuthDbContext _context;

        public AdminSeeder(AuthDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.Where(u => u.Email == "admin@gmail.com").AnyAsync()) return;
            
            await _context.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@gmail.com",
                PasswordHash = "$2a$11$rk0GEXJ7xGxmNOuOcpnL/e9cZ/jFi2Cs5WgXzL8Z0TVc09bTZcjeC",
                Avatar =
                    "https://res.cloudinary.com/dvyrpsngz/image/upload/v1772273537/products_images/xmbykvulquzh63jecnxe.png",
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
        }
    }