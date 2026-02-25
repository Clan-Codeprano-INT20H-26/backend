using Backend.Module.Tax.Domain;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Reflection;
using System.Text.Json; 

namespace Backend.Module.Tax.Infrastructure;

public class DataSeeder
{
    private readonly TaxDbContext _context;

    public DataSeeder(TaxDbContext context)
    {
        _context = context;
    }

    public async Task SeedAllAsync()
    {
        var basePath = AppContext.BaseDirectory;
        
        var countiesPath = Path.Combine(basePath, "Data", "new-york-counties.geojson");
        
        var dataDir = Path.Combine(basePath, "Data");
        var citiesFile = Directory.GetFiles(dataDir, "NYS_Civil_Boundaries_*.geojson").FirstOrDefault();
        
        await SeedCountiesAsync(countiesPath);
        
        if (citiesFile != null)
        {
            await SeedCitiesAsync(citiesFile);
        }
        else
        {
            Console.WriteLine($"[WARNING] City file not found in {dataDir}");
        }

        await SeedRatesAsync();
    }

    private async Task SeedCountiesAsync(string path)
    {
        if (await _context.Counties.AnyAsync()) return;

        if (!File.Exists(path))
        {
            Console.WriteLine($"[ERROR] File not found: {path}");
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(path);
            var features = new GeoJsonReader().Read<FeatureCollection>(json);
            var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var list = new List<County>();

            foreach (var feature in features)
            {
                // Проверь реальное название ключа в своём geojson файле!
                var nameKey = feature.Attributes.GetNames()
                    .FirstOrDefault(n => n.Equals("name", StringComparison.OrdinalIgnoreCase)
                                         || n.Equals("county_name", StringComparison.OrdinalIgnoreCase)
                                         || n.Equals("NAMELSAD", StringComparison.OrdinalIgnoreCase));

                if (nameKey == null)
                {
                    Console.WriteLine($"[WARNING] No name attribute found. Keys: {string.Join(", ", feature.Attributes.GetNames())}");
                    continue;
                }

                var name = feature.Attributes[nameKey]?.ToString();
                if (string.IsNullOrEmpty(name)) continue;

                if (TryGetMultiPolygon(feature.Geometry, factory, out var geom))
                {
                    list.Add(new County { Name = name, Geometry = geom });
                }
            }

            await _context.Counties.AddRangeAsync(list);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[INFO] Seeded {list.Count} counties.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to seed counties: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async Task SeedCitiesAsync(string path)
    {
        if (await _context.Cities.AnyAsync()) return;

        if (!File.Exists(path)) return;

        var json = await File.ReadAllTextAsync(path);
        var features = new GeoJsonReader().Read<FeatureCollection>(json);
        var factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var list = new List<City>();

        foreach (var feature in features)
        {
            var name = feature.Attributes["NAME"]?.ToString();
            

            if (TryGetMultiPolygon(feature.Geometry, factory, out var geom))
            {
                list.Add(new City { Name = name, Geometry = geom });
            }
        }
        
        if (list.Any())
        {
            await _context.Cities.AddRangeAsync(list);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[INFO] Seeded {list.Count} cities.");
        }
    }

    private async Task SeedRatesAsync()
    {
        if (await _context.TaxRates.AnyAsync()) return;

        var basePath = AppContext.BaseDirectory;
        var path = Path.Combine(basePath, "Data", "taxRates.json");

        if (!File.Exists(path))
        {
            Console.WriteLine($"[WARNING] Tax rates file not found at: {path}");
            return;
        }

        try 
        {
            var json = await File.ReadAllTextAsync(path);
        
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };
        
            var rates = JsonSerializer.Deserialize<List<TaxRate>>(json, options);

            if (rates != null && rates.Any())
            {
                await _context.TaxRates.AddRangeAsync(rates);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[INFO] Seeded {rates.Count} tax rates from JSON.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to seed tax rates: {ex.Message}");
        }
    }

    private bool TryGetMultiPolygon(Geometry input, GeometryFactory factory, out MultiPolygon result)
    {
        result = null!;
        if (input == null) return false;

        if (input is Polygon poly)
        {
            result = new MultiPolygon(new[] { poly }, factory) { SRID = 4326 };
            return true;
        }
        if (input is MultiPolygon mp)
        {
            mp.SRID = 4326;
            result = mp;
            return true;
        }
        return false;
    }
}