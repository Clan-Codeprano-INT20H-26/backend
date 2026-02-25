namespace Backend.Modules.Order.Application.Interfaces;

public interface IOrderBulkRepository
{
    Task BulkInsertOrdersAsync(IEnumerable<Domain.Order> orders, CancellationToken ct);
}