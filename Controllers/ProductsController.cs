using ECommerceMvc.Data;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMvc.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Details(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}
