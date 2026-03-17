using ECommerceMvc.Data;
using ECommerceMvc.Helpers;
using ECommerceMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMvc.Controllers;

public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }

    private IActionResult? RequireUserLogin()
    {
        if (!HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
        {
            var returnUrl = Url.Action("Checkout", "Order");
            return RedirectToAction("Login", "Auth", new { returnUrl });
        }

        return null;
    }

    public IActionResult Checkout()
    {
        var redirect = RequireUserLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var cart = SessionCartService.GetCart(HttpContext.Session);
        if (!cart.Any())
        {
            TempData["SuccessMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var model = new CheckoutViewModel
        {
            FullName = user.FullName,
            Address = user.Address,
            PhoneNumber = user.PhoneNumber,
            CartItems = cart,
            TotalPrice = cart.Sum(x => x.LineTotal)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Checkout(CheckoutViewModel model)
    {
        var redirect = RequireUserLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var userId = HttpContext.Session.GetInt32(SessionKeys.UserId)!.Value;
        var cart = SessionCartService.GetCart(HttpContext.Session);

        if (!cart.Any())
        {
            ModelState.AddModelError(string.Empty, "Your cart is empty.");
        }

        var cartProductIds = cart.Select(c => c.ProductId).Distinct().ToList();
        var products = _context.Products
            .Where(p => cartProductIds.Contains(p.Id))
            .ToDictionary(p => p.Id);

        foreach (var item in cart)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                ModelState.AddModelError(string.Empty, $"Product '{item.Name}' is no longer available.");
                continue;
            }

            if (item.Quantity > product.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"Not enough stock for '{item.Name}'. Available: {product.Quantity}.");
            }
        }

        if (!ModelState.IsValid)
        {
            model.CartItems = cart;
            model.TotalPrice = cart.Sum(x => x.LineTotal);
            return View(model);
        }

        var order = new Order
        {
            UserId = userId,
            FullName = model.FullName,
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            TotalPrice = cart.Sum(x => x.LineTotal),
            Status = "Pending",
            OrderItems = cart.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Price
            }).ToList()
        };

        foreach (var item in cart)
        {
            products[item.ProductId].Quantity -= item.Quantity;
        }

        _context.Orders.Add(order);
        _context.SaveChanges();

        SessionCartService.Clear(HttpContext.Session);

        return RedirectToAction("Success", new { id = order.Id });
    }

    public IActionResult Success(int id)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        return View(order);
    }
}
