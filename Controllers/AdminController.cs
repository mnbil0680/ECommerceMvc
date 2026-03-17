using ECommerceMvc.Data;
using ECommerceMvc.Helpers;
using ECommerceMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMvc.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin123";

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    private bool IsLoggedIn => HttpContext.Session.GetString(SessionKeys.IsAdminLoggedIn) == "true";

    private IActionResult? RequireLogin()
    {
        if (!IsLoggedIn)
        {
            return RedirectToAction("Login");
        }

        return null;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (IsLoggedIn)
        {
            return RedirectToAction("Products");
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string username, string password)
    {
        if (username == AdminUsername && password == AdminPassword)
        {
            HttpContext.Session.SetString(SessionKeys.IsAdminLoggedIn, "true");
            return RedirectToAction("Products");
        }

        ViewBag.ErrorMessage = "Invalid username or password.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(SessionKeys.IsAdminLoggedIn);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Users()
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var users = _context.Users
            .Include(u => u.Orders)
            .OrderByDescending(u => u.CreatedAt)
            .ToList();

        return View(users);
    }

    public IActionResult Products()
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var products = _context.Products.OrderBy(p => p.Name).ToList();
        return View(products);
    }

    [HttpGet]
    public IActionResult CreateProduct()
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(Product product)
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        _context.Products.Add(product);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Product created successfully.";
        return RedirectToAction("Products");
    }

    [HttpGet]
    public IActionResult EditProduct(int id)
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditProduct(int id, Product product)
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(product);
        }

        _context.Products.Update(product);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Product updated successfully.";
        return RedirectToAction("Products");
    }

    [HttpGet]
    public IActionResult DeleteProduct(int id)
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("DeleteProduct")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteProductConfirmed(int id)
    {
        var redirect = RequireLogin();
        if (redirect is not null)
        {
            return redirect;
        }

        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
        TempData["SuccessMessage"] = "Product deleted successfully.";

        return RedirectToAction("Products");
    }
}
