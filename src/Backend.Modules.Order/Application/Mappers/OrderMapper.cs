using Backend.Modules.Shared.DTOs.Order;

namespace Backend.Modules.Order.Mappers;

public static class OrderMapper
{
    public static OrderResponseDto ToDto(this Domain.Order order)
    {
        return new OrderResponseDto(
            order.Id,
            order.UserId,
            order.KitId,      
            order.SubTotal,
            order.Status.ToString(), 
            order.Latitude,
            order.Longitude,
            
            order.Taxes != null ? new TaxesBreakdownDto(
                order.Taxes.StateRate,
                order.Taxes.CountryRate,
                order.Taxes.CityRate,
                order.Taxes.SpecialRates,
                order.Taxes.Jurisdictions
            ) : null,

            order.TotalAmount 
        );
    }
}