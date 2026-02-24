namespace Backend.Module.Kit.Domain;

public class Kit
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Seller { get; set; } = string.Empty; 
    public decimal Price { get; set; }
    public List<string> Images { get; set; } = new(); 
}