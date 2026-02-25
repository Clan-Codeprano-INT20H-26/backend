using Backend.Module.Kit.Domain;
using Microsoft.EntityFrameworkCore;

namespace Backend.Module.Kit.Infrastructure;

public class KitDbContext : DbContext
{
    public DbSet<Domain.Kit> Kits { get; set; }

    public KitDbContext(DbContextOptions<KitDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Domain.Kit>(entity =>
        {
            entity.HasKey(k => k.Id);

            entity.Property(k => k.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(k => k.Price)
                .HasPrecision(18, 2); 
            
            entity.Property(k => k.Images)
                .HasColumnType("jsonb"); 
        });
    }
}