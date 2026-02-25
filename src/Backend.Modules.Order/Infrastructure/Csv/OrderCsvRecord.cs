using CsvHelper.Configuration.Attributes;

namespace Backend.Modules.Order.Infrastructure.Csv;

public class OrderCsvRecord
{
    [Name("kitId")] 
    public string KitIdsRaw { get; set; } = string.Empty;

    [Name("latitude")]
    public string Latitude { get; set; } = string.Empty;

    [Name("longitude")]
    public string Longitude { get; set; } = string.Empty;
}