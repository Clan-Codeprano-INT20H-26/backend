using FluentResults;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Pagination;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface IOrderService
{
    Task<Result<PagedResult<OrderResponseDto>>> GetAllAsync(Guid userId, OrderFilterDto filter);

    Task<Result<OrderResponseDto>> GetByIdAsync(Guid id, Guid userId);

    Task<Result<OrderResponseDto>> CreateOrderAsync(OrderCreateDto orderDto, Guid userId);

    Task<Result<OrderResponseDto>> UpdateOrderAsync(Guid id, OrderUpdateDto updateDto);

    Task<Result> DeleteOrderAsync(Guid id, Guid userId);
}