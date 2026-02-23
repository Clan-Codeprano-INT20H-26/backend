using System.ComponentModel.DataAnnotations;
using Backend.Modules.Shared.Models;

namespace Backend.Modules.Order.Domain;

public class Order : EntityBase
{
    public Guid UserId { get; set; }
    public List<Guid> KitId { get; set; }
    
    public decimal SubTotal { get; set; }
    
    public StatusOfOrder Status { get; set; }
    
    [MaxLength(255)]
    public string Latitude { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Longitude { get; set; } = string.Empty;

    public TaxesBreakdown? Taxes { get; set; }

    public decimal TotalAmount => SubTotal + (Taxes != null ? (Taxes.StateRate + Taxes.CountryRate + Taxes.CityRate + Taxes.SpecialRates) * SubTotal : 0);

    public Order() 
    {
        Taxes = new TaxesBreakdown();
    }

    public Order(Guid userId, List<Guid> kitId, decimal subTotal, string latitude, string longitude, TaxesBreakdown taxes)
    {
        UserId = userId;
        KitId = kitId;
        SubTotal = subTotal;
        Latitude = latitude;
        Longitude = longitude;
        
        Taxes = taxes ?? new TaxesBreakdown();
        
        Status = StatusOfOrder.Pending;
    }
}