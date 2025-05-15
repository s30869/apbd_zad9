using System.ComponentModel.DataAnnotations;

namespace Zadanie9.Model;

public class Order { 
    public int Id { get; set; } 
    [Required]
    public int IdProduct { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? FulfilledAt { get; set; }      
    
}