using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Backend.Module.Tax.Domain;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    
    [Column(TypeName = "geometry(MultiPolygon, 4326)")]
    public MultiPolygon Geometry { get; set; } = null!;
}