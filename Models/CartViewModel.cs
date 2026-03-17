namespace ECommerceMvc.Models;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}
