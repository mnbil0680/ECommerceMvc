using ECommerceMvc.Models;

namespace ECommerceMvc.Helpers;

public static class SessionCartService
{
    private const string CartKey = "Cart";

    public static List<CartItem> GetCart(ISession session)
    {
        return session.GetObject<List<CartItem>>(CartKey) ?? new List<CartItem>();
    }

    public static void SaveCart(ISession session, List<CartItem> cart)
    {
        session.SetObject(CartKey, cart);
    }

    public static void AddToCart(ISession session, Product product)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(x => x.ProductId == product.Id);

        if (item is null)
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Quantity = 1
            });
        }
        else
        {
            item.Quantity += 1;
        }

        SaveCart(session, cart);
    }

    public static void UpdateQuantity(ISession session, int productId, int quantity)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            return;
        }

        if (quantity <= 0)
        {
            cart.Remove(item);
        }
        else
        {
            item.Quantity = quantity;
        }

        SaveCart(session, cart);
    }

    public static void RemoveItem(ISession session, int productId)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item is not null)
        {
            cart.Remove(item);
            SaveCart(session, cart);
        }
    }

    public static void Clear(ISession session)
    {
        session.Remove(CartKey);
    }
}
