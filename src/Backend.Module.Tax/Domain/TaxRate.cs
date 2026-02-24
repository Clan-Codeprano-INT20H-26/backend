namespace Backend.Module.Tax.Domain;

public class TaxRate
{
    public int Id { get; set; }
    public string JurisdictionName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public string Type { get; set; } = string.Empty;
}