using System.Globalization;
using System.Runtime.CompilerServices;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using CsvHelper;
using CsvHelper.Configuration;

namespace Backend.Modules.Order.Infrastructure.Csv;

public class CsvParserService : ICsvParserService
{
    public async IAsyncEnumerable<CreateOrderRequest> ReadOrdersStreamAsync(Stream fileStream, [EnumeratorCancellation] CancellationToken ct)
    {
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var csv = new CsvReader(reader, config);

        while (await csv.ReadAsync())
        {
            if (ct.IsCancellationRequested) break;

            var record = csv.GetRecord<OrderCsvRecord>();
            
            if (record == null) continue;

            var items = new List<CreateOrderItemRequest>();

            var rawItems = record.KitIdsRaw?.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (rawItems != null)
            {
                foreach (var item in rawItems)
                {
                    var parts = item.Split(':');
                    
                    if (Guid.TryParse(parts[0], out var guid))
                    {
                        int quantity = 1;
                        if (parts.Length > 1 && int.TryParse(parts[1], out int parsedQuantity))
                        {
                            quantity = parsedQuantity; 
                        }

                        items.Add(new CreateOrderItemRequest(guid, quantity));
                    }
                }
            }
            
            if (!items.Any()) continue;

            yield return new CreateOrderRequest(items, record.Latitude, record.Longitude);
        }
    }
}