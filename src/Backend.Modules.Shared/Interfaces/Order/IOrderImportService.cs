using FluentResults;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface IOrderImportService
{
    Task<Result> ImportOrdersAsync(Guid userId, Stream fileStream, CancellationToken ct);
}