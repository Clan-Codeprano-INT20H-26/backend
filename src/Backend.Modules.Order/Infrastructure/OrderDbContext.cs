using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Order> Orders { get; set; }
}