using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zadanie9.Model;

public class ProductWarehouse
{
    public int Id { get; set; } 
    [Required]
    public int IdWarehouse { get; set; }

    [Required]
    public int IdProduct { get; set; }

    [Required]
    public int IdOrder { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}
