using Backend.Modules.Shared.Models;

namespace Backend.Modules.Order.Domain;

public class Order : EntityBase
{
    public Guid UserId { get; set; }
    public Guid KitId { get; set; }
    
    
    public decimal SubTotal { get; set; }
    
    public StatusOfOrder Status { get; set; }
    
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;

    public TaxesBreakdown? Taxes { get; set; }
    
}