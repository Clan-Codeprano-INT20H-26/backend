using Backend.Modules.Order.Application.Interfaces;
using Backend.Modules.Order.Infrastructure;
using Backend.Modules.Order.Mappers;
using Backend.Modules.Shared.Dto;
using Backend.Modules.Shared.Interfaces.Order;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Application;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _orderDbContext;
    private readonly ITaxHelper _taxHelper;

    public OrderService(OrderDbContext orderDbContext, ITaxHelper taxHelper)
    {
        _orderDbContext = orderDbContext;
        _taxHelper = taxHelper;
    }

    public async Task<Result<List<OrderResponseDto>>> GetAllAsync()
    {
        var orders = await _orderDbContext.Orders.ToListAsync();
        
        var dtos = orders.Select(o => o.ToDto()).ToList();

        return Result.Ok(dtos);
    }

    public async Task<Result<OrderResponseDto>> GetByIdAsync(Guid id)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return Result.Fail($"Order with id {id} not found");
        }

        // Возвращаем DTO
        return Result.Ok(order.ToDto());
    }

    // --- CREATE ---
    public async Task<Result<OrderResponseDto>> CreateOrderAsync(OrderCreateDto orderDto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(orderDto.latitude) || string.IsNullOrWhiteSpace(orderDto.longitude))
        {
            return Result.Fail("Latitude and Longitude are required.");
        }

        try
        {
            // TODO: calculate subtotal from kits prices (заглушка 100.00m)
            var taxes = await _taxHelper.GetTaxesBreakdownAsync(orderDto.latitude, orderDto.longitude);
            
            var newOrder = new Domain.Order(
                userId, 
                orderDto.kitId,
                100.00m, 
                orderDto.latitude, 
                orderDto.longitude, 
                taxes
            );

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

        if (order is null)
        {
            return Result.Fail($"Order with id {id} not found");
        }

        try
        {//TODO imporove
            _orderDbContext.Orders.Update(order);
            await _orderDbContext.SaveChangesAsync();
            
            return Result.Ok(order.ToDto());
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error("Failed to update order").CausedBy(ex));
        }
    }

    public async Task<Result> DeleteOrderAsync(Guid id)
    {
        var order = await _orderDbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return Result.Fail($"Order with id {id} not found");
        }

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