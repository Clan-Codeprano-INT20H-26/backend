using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Order.Mappers;
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

    public async Task<Result<PagedResponse<OrderResponse>>> GetAllAsync(Guid userId, OrderFilterRequest filter)
    {
        var query = _orderDbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId);

        if (filter.FromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            var toDate = filter.ToDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(o => o.CreatedAt <= toDate);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(o => o.TotalAmount >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(o => o.TotalAmount <= filter.MaxPrice.Value);
        }

        query = query.OrderByDescending(o => o.CreatedAt); 
        
        var totalCount = await query.CountAsync();

        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(); 

        var dtos = items.Select(o => o.ToDto()).ToList();

        var result = new PagedResponse<OrderResponse>(dtos, totalCount, pageNumber, pageSize);

        return Result.Ok(result);
    }

    public async Task<Result<OrderResponse>> GetByIdAsync(Guid id, Guid userId)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return Result.Fail($"Order with id {id} not found");
        }

        if (order.UserId != userId)
        {
            return Result.Fail($"User with id {userId} is not the current user");
        }
        return Result.Ok(order.ToDto());
    }

    public async Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest createOrderDto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(createOrderDto.latitude) || string.IsNullOrWhiteSpace(createOrderDto.longitude))
        {
            return Result.Fail("Latitude and Longitude are required.");
        }

        if (!decimal.TryParse(createOrderDto.latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat) ||
            !decimal.TryParse(createOrderDto.longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon))
        {
            return Result.Fail("Invalid coordinate format.");
        }

        try
        {
            var result = await _kitService.CalculateTotalPriceAsync(createOrderDto.kitPacks);

            if (!result.IsSuccess)
            {
                return Result.Fail(result.Errors.First().Message);
            }

            var subTotal = result.Value;
            
            var taxResult = await _taxHelper.CalculateTaxesAsync(lat, lon);

            if (taxResult.IsFailed)
            {
                return Result.Fail(taxResult.Errors.First().Message); 
            }

            var taxDto = taxResult.Value;

            var taxDomain = new TaxesBreakdown
            {
                StateRate = taxDto.StateRate,
                CountryRate = taxDto.CountyRate,
                CityRate = taxDto.CityRate,
                SpecialRates = taxDto.SpecialRates,
                Jurisdictions = taxDto.Jurisdictions ?? new List<string>()
            };
            
            var newOrder = new Domain.Order(
                userId, 
                OrderItemMapper.ToDomains(createOrderDto.kitPacks),
                subTotal, 
                createOrderDto.latitude, 
                createOrderDto.longitude
            );

            newOrder.ApplyTax(taxDomain);

            await _orderDbContext.Orders.AddAsync(newOrder);
            await _orderDbContext.SaveChangesAsync();

            return Result.Ok(newOrder.ToDto());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to create order").CausedBy(ex));
        }
    }

    public async Task<Result<OrderResponse>> UpdateOrderAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return Result.Fail($"Order with id {id} not found");

        try
        {
            _orderDbContext.Orders.Update(order);
            await _orderDbContext.SaveChangesAsync();
            
            return Result.Ok(order.ToDto());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to update order").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteOrderAsync(Guid id, Guid userId)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return Result.Fail($"Order with id {id} not found");

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
}