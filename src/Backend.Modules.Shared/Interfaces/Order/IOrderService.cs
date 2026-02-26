using FluentResults;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Pagination;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface IOrderService
{
    Task<Result<PagedResponse<OrderResponse>>> GetAllAsync(Guid userId, OrderFilterRequest filter);

    Task<Result<OrderResponse>> GetByIdAsync(Guid id, Guid userId);

    Task<Result<OrderResponse>> CreateOrderAsync(CreateOrderRequest createOrderDto, Guid userId);

    Task<Result<OrderResponse>> UpdateOrderAsync(Guid id, UpdateOrderRequest request);

    Task<Result> DeleteOrderAsync(Guid id, Guid userId);
}