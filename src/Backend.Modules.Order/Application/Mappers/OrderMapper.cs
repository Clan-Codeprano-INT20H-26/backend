using System.Linq;
using System.Collections.Generic;
using Backend.Modules.Order.Domain;
using Backend.Modules.Shared.DTOs.Order;
using Backend.Modules.Shared.DTOs.Tax;
using Backend.Modules.Shared.DTOs.Kit;

namespace Backend.Modules.Order.Application.Mappers;

public static class OrderMapper
{
    public static OrderResponse ToDto(this Domain.Order entity, List<KitResponse> kits)
    {
        if (entity == null) return null!;

        return new OrderResponse(
            entity.Id,
            entity.Items.Select(i => new OrderItemResponse(
                kits.FirstOrDefault(k => k.Id == i.KitId)!,
                i.Quantity
            )).ToList(), 
            entity.SubTotal,
            entity.Status.ToString(),
            entity.Latitude,
            entity.Longitude,
            entity.TaxAmount,
            entity.CompositeTaxRate,
            entity.TotalAmount,
            entity.CreatedAt,
            entity.Taxes != null ? new TaxBreakdownResponse(
                entity.Taxes.StateRate,
                entity.Taxes.CountryRate,
                entity.Taxes.CityRate,
                entity.Taxes.SpecialRates,
                entity.Taxes.Jurisdictions
            ) : null
        );
    }
}