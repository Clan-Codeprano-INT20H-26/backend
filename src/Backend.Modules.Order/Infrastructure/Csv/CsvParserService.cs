using System.Globalization;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using CsvHelper;
using CsvHelper.Configuration;

namespace Backend.Modules.Order.Infrastructure.Csv;

public class CsvParserService : ICsvParserService
{
    public async IAsyncEnumerable<OrderCreateDto> ReadOrdersStreamAsync(Stream fileStream, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvReader(reader, config);

        while (await csv.ReadAsync())
        {
            ct.ThrowIfCancellationRequested();
            var record = csv.GetRecord<OrderCsvRecord>();
            
            if (record == null) continue;

            var kitIds = record.KitIdsRaw
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => Guid.TryParse(id, out var guid) ? guid : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();

            yield return new OrderCreateDto
            {
                kitId = kitIds,
                latitude = record.Latitude,
                longitude = record.Longitude
            };
        }
    }
}