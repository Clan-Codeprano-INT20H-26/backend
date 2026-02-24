using Backend.Module.Tax.Domain;
using Microsoft.EntityFrameworkCore;

namespace Backend.Module.Tax.Infrastructure;

public class TaxDbContext : DbContext
{
    public TaxDbContext(DbContextOptions<TaxDbContext> options) : base(options) { }

    public DbSet<County> Counties { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<TaxRate> TaxRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("postgis");
        
        modelBuilder.Entity<TaxRate>()
            .Property(p => p.Rate)
            .HasPrecision(10, 5); 

        base.OnModelCreating(modelBuilder);
    }
}