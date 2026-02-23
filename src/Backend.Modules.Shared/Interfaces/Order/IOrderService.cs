using FluentResults;
using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface IOrderService
{
    Task<Result<List<OrderResponseDto>>> GetAllAsync();

    Task<Result<OrderResponseDto>> GetByIdAsync(Guid id);

    Task<Result<OrderResponseDto>> CreateOrderAsync(OrderCreateDto orderDto, Guid userId);

    Task<Result<OrderResponseDto>> UpdateOrderAsync(Guid id, OrderUpdateDto updateDto);

    Task<Result> DeleteOrderAsync(Guid id);
}