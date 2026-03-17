using System.ComponentModel.DataAnnotations;

namespace ECommerceMvc.Models;

public class CheckoutViewModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    public List<CartItem> CartItems { get; set; } = new();

    public decimal TotalPrice { get; set; }
}
