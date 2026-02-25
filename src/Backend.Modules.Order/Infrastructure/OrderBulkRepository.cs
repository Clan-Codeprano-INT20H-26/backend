using Backend.Modules.Order.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

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
        var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
        var wasClosed = conn.State == System.Data.ConnectionState.Closed;
        if (wasClosed) await conn.OpenAsync(ct);

        var copyCommand = @"COPY ""Orders"" (
            ""Id"", ""CreatedAt"",
            ""UserId"", ""KitId"", ""SubTotal"", ""Status"", ""Latitude"", ""Longitude"", 
            ""Taxes_StateRate"", ""Taxes_CountryRate"", ""Taxes_CityRate"", ""Taxes_SpecialRates"", ""Taxes_Jurisdictions"", 
            ""CompositeTaxRate"", ""TaxAmount"", ""TotalAmount""
        ) FROM STDIN (FORMAT BINARY)";

        using (var writer = await conn.BeginBinaryImportAsync(copyCommand, ct))
        {
            foreach (var order in orders)
            {
                await writer.StartRowAsync(ct);

                await writer.WriteAsync(order.Id, NpgsqlDbType.Uuid, ct);
                await writer.WriteAsync(order.CreatedAt, NpgsqlDbType.TimestampTz, ct);
                await writer.WriteAsync(order.UserId, NpgsqlDbType.Uuid, ct);
                await writer.WriteAsync(order.KitId.ToArray(), NpgsqlDbType.Array | NpgsqlDbType.Uuid, ct);
                await writer.WriteAsync(order.SubTotal, NpgsqlDbType.Numeric, ct);
                await writer.WriteAsync((int)order.Status, NpgsqlDbType.Integer, ct);
                await writer.WriteAsync(order.Latitude, NpgsqlDbType.Varchar, ct);
                await writer.WriteAsync(order.Longitude, NpgsqlDbType.Varchar, ct);

                if (order.Taxes != null)
                {
                    await writer.WriteAsync(order.Taxes.StateRate, NpgsqlDbType.Numeric, ct);
                    await writer.WriteAsync(order.Taxes.CountryRate, NpgsqlDbType.Numeric, ct);
                    await writer.WriteAsync(order.Taxes.CityRate, NpgsqlDbType.Numeric, ct);
                    await writer.WriteAsync(order.Taxes.SpecialRates, NpgsqlDbType.Numeric, ct);
                    await writer.WriteAsync(order.Taxes.Jurisdictions.ToArray(), NpgsqlDbType.Array | NpgsqlDbType.Text, ct);
                }
                else
                {
                    await writer.WriteNullAsync(ct); await writer.WriteNullAsync(ct);
                    await writer.WriteNullAsync(ct); await writer.WriteNullAsync(ct);
                    await writer.WriteNullAsync(ct);
                }

                await writer.WriteAsync(order.CompositeTaxRate, NpgsqlDbType.Numeric, ct);
                await writer.WriteAsync(order.TaxAmount, NpgsqlDbType.Numeric, ct);
                await writer.WriteAsync(order.TotalAmount, NpgsqlDbType.Numeric, ct);
            }
            await writer.CompleteAsync(ct);
        }

        if (wasClosed) await conn.CloseAsync();
    }
}