using Backend.Modules.Order.Application.Mappers;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Tax;

namespace Backend.Modules.Order.Mappers;

public static class OrderMapper
{
    public static OrderResponseDto ToDto(this Domain.Order order)
    {
        return new OrderResponseDto(
            order.Id,
            order.UserId,
            KitPackMapper.ToDtos(order.KitPacks),      
            order.SubTotal,
            order.Status.ToString(), 
            order.Latitude,
            order.Longitude,
            order.TaxAmount,
            order.CompositeTaxRate,
            order.TotalAmount,
            order.Taxes != null ? new TaxesBreakdownDto(
                order.Taxes.StateRate,
                order.Taxes.CountryRate,
                order.Taxes.CityRate,
                order.Taxes.SpecialRates,
                order.Taxes.Jurisdictions
            ) : null
        );
    }
}