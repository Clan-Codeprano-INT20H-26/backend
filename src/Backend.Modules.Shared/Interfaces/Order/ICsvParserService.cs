using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface ICsvParserService
{
    IAsyncEnumerable<CreateOrderRequest> ReadOrdersStreamAsync(Stream fileStream, CancellationToken ct);
}