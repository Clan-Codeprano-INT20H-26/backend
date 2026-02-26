using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Backend.Modules.Order.Domain;

[Owned]
public class OrderItem
{
    [Required]
    public Guid KitId { get; set; }
    [Required]
    public int Quantity { get; set; }
}