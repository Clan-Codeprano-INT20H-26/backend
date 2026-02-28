using System.Text.Json;
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

        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "kits.json");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[WARN] Seeding error: {filePath}");
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var kits = JsonSerializer.Deserialize<List<Domain.Kit>>(json, options);

        if (kits != null && kits.Any())
        {
            await _context.Kits.AddRangeAsync(kits);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[INFO] Seeded {kits.Count} kits from kits.json.");
        }
        else
        {
            Console.WriteLine("[WARN] empty  kits.json.");
        }
    }
}