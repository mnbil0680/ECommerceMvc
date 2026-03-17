using System.ComponentModel.DataAnnotations;

namespace ECommerceMvc.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    [Range(0, 1000000)]
    public int Quantity { get; set; }

    [Display(Name = "Image URL")]
    [Required]
    [Url]
    public string ImageUrl { get; set; } = string.Empty;
}
