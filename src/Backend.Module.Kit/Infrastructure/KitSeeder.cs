using Backend.Module.Kit.Domain;
using Microsoft.EntityFrameworkCore;

namespace Backend.Module.Kit.Infrastructure;

public class KitSeeder
{
    private readonly KitDbContext _context;

    public KitSeeder(KitDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.Kits.AnyAsync()) return;

        var kits = new List<Domain.Kit>();

        for (int i = 0; i <= 99; i++)
        {
            // CSV has IDs like: 550e8400-e29b-41d4-a716-446655440000
            // The last segment is the decimal number as a literal string, zero-padded to 12 chars
            var id = Guid.Parse($"550e8400-e29b-41d4-a716-{(446655440000L + i):D12}");
            kits.Add(new Domain.Kit
            {
                Id = id,
                Name = $"Kit {i}",
                Description = $"Seeded kit {i}",
                Seller = "Seeder",
                Price = 10.00m + i,
                Images = new List<string>()
            });
        }

        await _context.Kits.AddRangeAsync(kits);
        await _context.SaveChangesAsync();
        Console.WriteLine($"[INFO] Seeded {kits.Count} kits.");
    }
}