using ECommerceMvc.Data;
using ECommerceMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var products = _context.Products.OrderBy(p => p.Name).ToList();
        return View(products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
