using System.ComponentModel.DataAnnotations;

namespace Zadanie9.Model;

public class Warehouse
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public string Address { get; set; }
}