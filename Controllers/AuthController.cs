using ECommerceMvc.Data;
using ECommerceMvc.Helpers;
using ECommerceMvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMvc.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (_context.Users.Any(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email.Trim().ToLowerInvariant(),
            PhoneNumber = model.PhoneNumber,
            Address = model.Address,
            PasswordHash = PasswordHasher.Hash(model.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
        HttpContext.Session.SetString(SessionKeys.UserName, user.FullName);

        TempData["SuccessMessage"] = "Account created successfully.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionKeys.UserId).HasValue)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim().ToLowerInvariant();
        var passwordHash = PasswordHasher.Hash(model.Password);
        var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == passwordHash);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeys.UserId, user.Id);
        HttpContext.Session.SetString(SessionKeys.UserName, user.FullName);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove(SessionKeys.UserId);
        HttpContext.Session.Remove(SessionKeys.UserName);
        TempData["SuccessMessage"] = "You have been logged out.";

        return RedirectToAction("Index", "Home");
    }
}