using FluentResults;
using Backend.Modules.Shared.Dto;

namespace Backend.Modules.Shared.Interfaces.Order;

public interface IOrderService
{
    /// <summary>
    /// Получить все заказы
    /// </summary>
    Task<Result<List<OrderResponseDto>>> GetAllAsync();

    /// <summary>
    /// Получить заказ по ID
    /// </summary>
    Task<Result<OrderResponseDto>> GetByIdAsync(Guid id);

    /// <summary>
    /// Создать новый заказ
    /// </summary>
    Task<Result<OrderResponseDto>> CreateOrderAsync(OrderCreateDto orderDto, Guid userId);

    /// <summary>
    /// Обновить существующий заказ
    /// </summary>
    Task<Result<OrderResponseDto>> UpdateOrderAsync(Guid id, OrderUpdateDto updateDto);

    /// <summary>
    /// Удалить заказ
    /// </summary>
    Task<Result> DeleteOrderAsync(Guid id);
}