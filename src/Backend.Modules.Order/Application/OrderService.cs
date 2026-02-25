using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Order.Mappers;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.Interfaces.Order;
using Backend.Modules.Shared.Interfaces.Tax;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Backend.Modules.Order.Domain;
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

    public async Task<Result<List<OrderResponseDto>>> GetAllAsync(Guid userId)
    {
        var orders = await _orderDbContext.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .ToListAsync();
        
        var dtos = orders.Select(o => o.ToDto()).ToList();

        return Result.Ok(dtos);
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(Guid id, Guid userId)
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

    public async Task<Result<OrderResponseDto>> CreateOrderAsync(OrderCreateDto orderDto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(orderDto.latitude) || string.IsNullOrWhiteSpace(orderDto.longitude))
        {
            return Result.Fail("Latitude and Longitude are required.");
        }

        if (!decimal.TryParse(orderDto.latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lat) ||
            !decimal.TryParse(orderDto.longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal lon))
        {
            return Result.Fail("Invalid coordinate format.");
        }

        try
        {
            var result = await _kitService.CalculateTotalPriceAsync(orderDto.kitId);

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
                orderDto.kitId,
                subTotal, 
                orderDto.latitude, 
                orderDto.longitude
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

    public async Task<Result<OrderResponseDto>> UpdateOrderAsync(Guid id, OrderUpdateDto updateDto)
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