using System.Globalization;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using CsvHelper;
using CsvHelper.Configuration;

namespace Backend.Modules.Order.Infrastructure.Csv;

public class CsvParserService : ICsvParserService
{
    public async IAsyncEnumerable<CreateOrderRequest> ReadOrdersStreamAsync(Stream fileStream, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvReader(reader, config);

        while (await csv.ReadAsync())
        {
            ct.ThrowIfCancellationRequested();
            var record = csv.GetRecord<OrderCsvRecord>();
            
            if (record == null) continue;

            var items = new List<OrderItemDto>();

            var rawItems = record.KitIdsRaw.Split('|', StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in rawItems)
            {
                var parts = item.Split(':');
                
                if (Guid.TryParse(parts[0], out var guid))
                {
                    int count = 1;
                    if (parts.Length > 1 && int.TryParse(parts[1], out int parsedCount))
                    {
                        count = parsedCount;
                    }

                    items.Add(new OrderItemDto(guid, count));
                }
            }
            
            if (!items.Any()) continue;

            yield return new CreateOrderRequest(items, record.Latitude, record.Longitude);
        }
    }
}