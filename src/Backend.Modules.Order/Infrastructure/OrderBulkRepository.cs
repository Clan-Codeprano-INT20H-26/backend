using Backend.Modules.Order.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Infrastructure;

public class OrderBulkRepository : IOrderBulkRepository
{
    private readonly OrderDbContext _context;

    public OrderBulkRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task BulkInsertOrdersAsync(IEnumerable<Domain.Order> orders, CancellationToken ct)
    {
        _context.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            await _context.Orders.AddRangeAsync(orders, ct);
            await _context.SaveChangesAsync(ct);
        }
        finally
        {
            _context.ChangeTracker.Clear();
            _context.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}