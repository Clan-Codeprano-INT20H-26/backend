using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.SomeEntity.Infrastructure;

public class SomeEntityDbContext : DbContext
{
    public SomeEntityDbContext(DbContextOptions<SomeEntityDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.SomeEntity> SomeEntities { get; set; }
}