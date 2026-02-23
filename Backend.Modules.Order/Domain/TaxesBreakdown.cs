using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Domain;
[Owned]
public class TaxesBreakdown
{
    public decimal StateRate { get; set; }
    public decimal CountryRate { get; set; }
    public decimal CityRate { get; set; }
    public decimal SpecialRates { get; set; }
    
    public List<string> Jurisdictions { get; set; } = new();
}