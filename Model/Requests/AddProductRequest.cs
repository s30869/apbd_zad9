using System.ComponentModel.DataAnnotations;

namespace Zadanie9.Model.Requests;

public class AddProductRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int WarehouseId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}