using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Order>(builder =>
        {
            builder.HasKey(o => o.Id);

            builder.OwnsMany(o => o.KitPacks, kp =>
            {
                kp.ToTable("KitPack");
                kp.WithOwner().HasForeignKey("OrderId");
                kp.HasKey("OrderId", "KitId"); 

                kp.Property(p => p.KitId).IsRequired();
                kp.Property(p => p.Count).IsRequired();
            });

          
            builder.OwnsOne(o => o.Taxes, t =>
            {
                t.Property(x => x.StateRate).HasColumnType("decimal(18,4)");
                t.Property(x => x.CountryRate).HasColumnType("decimal(18,4)"); 
                t.Property(x => x.CityRate).HasColumnType("decimal(18,4)");
                t.Property(x => x.SpecialRates).HasColumnType("decimal(18,4)");
                
                t.Property(x => x.Jurisdictions).HasColumnType("text[]");
            });
            
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(o => o.CompositeTaxRate).HasColumnType("decimal(18,4)");
            
            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => o.CreatedAt);
        });
    }
}