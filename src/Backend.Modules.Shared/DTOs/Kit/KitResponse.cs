namespace Backend.Modules.Shared.DTOs.Kit;

public class KitResponse
{
    public Guid id { get; set; } 
    public string name { get; set; }
    public string description { get; set; }
    public string seller { get; set; }
    public decimal price { get; set; }
    
    public List<string> images { get; set; }
}