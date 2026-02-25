using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Domain;

[Owned]
public class KitPack
{
    [Required]
    public Guid KitId { get; set; }
    [Required]
    public int Count { get; set; }
}