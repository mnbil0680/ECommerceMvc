using ECommerceMvc.Data;
using ECommerceMvc.Helpers;
using ECommerceMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMvc.Controllers;

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = SessionCartService.GetCart(HttpContext.Session);
        var model = new CartViewModel
        {
            Items = cart,
            TotalPrice = cart.Sum(x => x.LineTotal)
        };

        return View(model);
    }

    private IActionResult? RequireUserLogin()
    {
        if (!HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
        {
            var returnUrl = Url.Action("Index", "Cart");
            return RedirectToAction("Login", "Auth", new { returnUrl });
        }

        return null;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int productId)
    {
        var redirect = RequireUserLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            return NotFound();
        }

        var cart = SessionCartService.GetCart(HttpContext.Session);
        var currentQuantity = cart.FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;
        if (currentQuantity >= product.Quantity)
        {
            TempData["SuccessMessage"] = "No more stock available for this product.";
            return RedirectToAction("Index", "Home");
        }

        SessionCartService.AddToCart(HttpContext.Session, product);
        TempData["SuccessMessage"] = "Product added to cart.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateQuantity(int productId, int quantity)
    {
        var redirect = RequireUserLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            return NotFound();
        }

        if (quantity > product.Quantity)
        {
            quantity = product.Quantity;
            TempData["SuccessMessage"] = "Quantity adjusted to available stock.";
        }

        SessionCartService.UpdateQuantity(HttpContext.Session, productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        SessionCartService.RemoveItem(HttpContext.Session, productId);
        TempData["SuccessMessage"] = "Product removed from cart.";

        return RedirectToAction("Index");
    }
}
