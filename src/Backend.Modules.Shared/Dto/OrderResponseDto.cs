namespace Backend.Modules.Shared.Dto;

public record OrderResponseDto(
    Guid Id,                    // ID заказа (обычно он есть в базовом классе сущности)
    Guid UserId,
    List<Guid> KitIds,          // Переименовал в KitIds (мн. число), так правильнее для списка
    decimal SubTotal,
    string Status,              // Enum превращаем в строку
    string Latitude,
    string Longitude,
    TaxesBreakdownDto? Taxes,   // Вложенная DTO
    decimal TotalAmount         // Сюда запишем уже посчитанный результат
);