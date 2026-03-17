using ECommerceMvc.Data;
using ECommerceMvc.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMvc.Controllers;

public class OrdersController : Controller
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    private IActionResult? RequireAdminLogin()
    {
        // Keep admin auth simple for MVP: rely on session flag set by AdminController login.
        if (HttpContext.Session.GetString(SessionKeys.IsAdminLoggedIn) != "true")
        {
            return RedirectToAction("Login", "Admin");
        }

        return null;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var redirect = RequireAdminLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var orders = _context.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        return View(orders);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var redirect = RequireAdminLogin();
        if (redirect is not null)
        {
            return redirect;
        }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(int id, string status)
    {
        var redirect = RequireAdminLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        // Restrict status updates to a simple fixed list.
        var allowedStatuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };
        if (!allowedStatuses.Contains(status))
        {
            TempData["SuccessMessage"] = "Invalid order status.";
            return RedirectToAction("Details", new { id });
        }

        var order = _context.Orders.FirstOrDefault(o => o.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        order.Status = status;
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Order status updated.";

        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var redirect = RequireAdminLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var order = _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefault(o => o.Id == id);

        if (order is null)
        {
            return NotFound();
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Order deleted successfully.";

        return RedirectToAction("Index");
    }
}
