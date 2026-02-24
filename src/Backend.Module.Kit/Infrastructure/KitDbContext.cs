using Microsoft.EntityFrameworkCore;

namespace Backend.Module.Kit.Infrastructure;

public class KitDbContext : DbContext
{
    public KitDbContext(DbContextOptions<KitDbContext> options) : base(options) { }

    public DbSet<Domain.Kit> Kits => Set<Domain.Kit>();
}