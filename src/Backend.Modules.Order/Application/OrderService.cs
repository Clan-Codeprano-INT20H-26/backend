using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using Backend.Modules.Shared.Interfaces.Tax;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Backend.Modules.Order.Application.Mappers;
using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.DTOs.Pagination;
using Backend.Modules.Shared.Interfaces.Kit;
using Backend.Modules.Shared.DTOs.Kit;

namespace Backend.Modules.Order.Application;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _orderDbContext;
    private readonly ITaxService _taxHelper;
    private readonly IKitService _kitService;

    public OrderService(OrderDbContext orderDbContext, ITaxService taxHelper, IKitService kitService)
    {
        _orderDbContext = orderDbContext;
        _taxHelper = taxHelper;
        _kitService = kitService;
    }

    public async Task<Result<PagedResponse<OrderResponse>>> GetAllAsync(Guid userId, bool isAdmin, OrderFilterRequest filter)
    {
        var query = _orderDbContext.Orders
            .Include(o => o.Items)
            .AsNoTracking();

        if (!isAdmin)
        {
            query = query.Where(o => o.UserId == userId);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(o => o.TotalAmount >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(o => o.TotalAmount <= filter.MaxPrice.Value);
        }
        
        var totalCount = await query.CountAsync();
        
        var sortBy = filter.SortBy?.ToLower()?.Trim();
        var isDesc = filter.IsDescending;

        query = sortBy switch
        {
            "price" => isDesc ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
            "date" => isDesc ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
            _ => query.OrderByDescending(o => o.CreatedAt)
        };

        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ?  5 : filter.PageSize;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); 
        
        var allKitIds = items.SelectMany(o => o.Items).Select(i => i.KitId).ToList();
        var allKits = await FetchKitsAsync(allKitIds);

        var dtos = items.Select(o => o.ToDto(allKits)).ToList();

        var result = new PagedResponse<OrderResponse>(dtos, totalCount, pageNumber, pageSize);

        return Result.Ok(result);
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(Guid id, Guid userId)
    {
        var order = await _orderDbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return Result.Fail($"Order with id {id} not found");
        if (order.UserId != userId) return Result.Fail($"User with id {userId} is not the current user");
        
        var kitIds = order.Items.Select(i => i.KitId).ToList();
        var kits = await FetchKitsAsync(kitIds);

        return Result.Ok(order.ToDto(kits));
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest createOrderDto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(createOrderDto.Latitude) || string.IsNullOrWhiteSpace(createOrderDto.Longitude))
        {
            return Result.Fail("Latitude and Longitude are required.");
        }

        if (!decimal.TryParse(createOrderDto.Latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat) ||
            !decimal.TryParse(createOrderDto.Longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon))
        {
            return Result.Fail("Invalid coordinate format.");
        }

        try
        {
            var result = await _kitService.CalculateTotalPriceAsync(createOrderDto.Items);

            if (!result.IsSuccess) return Result.Fail(result.Errors.First().Message);

            var subTotal = result.Value;
            var taxResult = await _taxHelper.CalculateTaxesAsync(lat, lon);

            if (taxResult.IsFailed) return Result.Fail(taxResult.Errors.First().Message); 

            var taxDto = taxResult.Value;

            var taxDomain = new TaxesBreakdown
            {
                StateRate = taxDto.StateRate,
                CountryRate = taxDto.CountyRate,
                CityRate = taxDto.CityRate,
                SpecialRates = taxDto.SpecialRates,
                Jurisdictions = taxDto.Jurisdictions?.ToList() ?? new List<string>()
            };
            
            var newOrder = new Domain.Order(
                userId, 
                OrderItemMapper.ToDomains(createOrderDto.Items),
                subTotal, 
                createOrderDto.Latitude, 
                createOrderDto.Longitude
            );

            newOrder.ApplyTax(taxDomain);

            await _orderDbContext.Orders.AddAsync(newOrder);
            await _orderDbContext.SaveChangesAsync();

            var kitIds = newOrder.Items.Select(i => i.KitId).ToList();
            var kits = await FetchKitsAsync(kitIds);

            return Result.Ok(newOrder.ToDto(kits));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to create order").CausedBy(ex));
        }
    }

    public async Task<Result<OrderResponse>> UpdateOrderAsync(Guid orderId, Guid userId, UpdateOrderRequest request)
    {
        var order = await _orderDbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null) return Result.Fail($"Order with id {orderId} not found");
        if (order.UserId != userId) return Result.Fail($"User with id {userId} is not the current user");
        
        try
        {
            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<StatusOfOrder>(request.Status, true, out var newStatus))
            {
                order.Status = newStatus;
            }
            
            if (request.Latitude != null) order.Latitude = request.Latitude;
            if (request.Longitude != null) order.Longitude = request.Longitude;

            _orderDbContext.Orders.Update(order);
            await _orderDbContext.SaveChangesAsync();
            
            var kitIds = order.Items.Select(i => i.KitId).ToList();
            var kits = await FetchKitsAsync(kitIds);

            return Result.Ok(order.ToDto(kits));
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to update order").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteOrderAsync(Guid orderId, Guid userId)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null) return Result.Fail($"Order with id {orderId} not found");
        if(order.UserId != userId) return Result.Fail($"User with id {userId} is not the current user");
        
        try
        {
            _orderDbContext.Orders.Remove(order);
            await _orderDbContext.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to delete order").CausedBy(ex));
        }
    }

    private async Task<List<KitResponse>> FetchKitsAsync(IEnumerable<Guid> kitIds)
    {
        if (kitIds == null || !kitIds.Any())
        {
            return new List<KitResponse>();
        }

        var distinctIds = kitIds.Distinct().ToList();
        
        var kitsResult = await _kitService.GetByIdsAsync(distinctIds);

        if (kitsResult.IsSuccess)
        {
            return kitsResult.Value;
        }

        return new List<KitResponse>();
    }
}