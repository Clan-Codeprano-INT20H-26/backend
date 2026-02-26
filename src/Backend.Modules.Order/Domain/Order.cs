using System.ComponentModel.DataAnnotations;
using Backend.Modules.Shared.Models;

namespace Backend.Modules.Order.Domain;

public class Order : EntityBase
{
    public Guid UserId { get; set; }
    
    public List<OrderItem> Items { get; set; } = new();
    
    public decimal SubTotal { get; private set; }
    
    public StatusOfOrder Status { get; set; }
    
    [MaxLength(255)]
    public string Latitude { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Longitude { get; set; } = string.Empty;

    public TaxesBreakdown? Taxes { get; private set; }

    public decimal TaxAmount { get; private set; }
    public decimal CompositeTaxRate { get; private set; }
    public decimal TotalAmount { get; private set; }
    
    public Order() 
    {
        Taxes = new TaxesBreakdown();
    }

    public Order(Guid userId, List<OrderItem> items, decimal subTotal, string latitude, string longitude)
    {
        UserId = userId;
        Items = items;
        SubTotal = subTotal;
        Latitude = latitude;
        Longitude = longitude;
        Status = StatusOfOrder.Pending;
        
        TaxAmount = 0;
        TotalAmount = subTotal; 
        CompositeTaxRate = 0;
    }
    
    public void ApplyTax(TaxesBreakdown taxes)
    {
        if (taxes == null) throw new ArgumentNullException(nameof(taxes));

        Taxes = taxes;
        
        CompositeTaxRate = taxes.StateRate + taxes.CountryRate + taxes.CityRate + taxes.SpecialRates;
        
        TaxAmount = Math.Round(SubTotal * CompositeTaxRate, 2, MidpointRounding.AwayFromZero);

        TotalAmount = SubTotal + TaxAmount;
    }
}