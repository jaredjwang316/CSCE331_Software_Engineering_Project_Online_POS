using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp.Controllers;

public class LoginController : Controller
{
    //private readonly ILogger<LoginController> _logger;

    // public LoginController(ILogger<LoginController> logger)
    // {
    //     _logger = logger;
    // }

    #if DEBUG
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = "https://localhost:5001/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Google");
    }
    #else
    [AllowAnonymous]
    public IActionResult Login(string returnUrl = "https://07r-webapp.azurewebsites.net/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Google");
    }
    #endif

    public async Task<IActionResult> Logout()
    {
        Console.WriteLine("LoginController.Logout() called.");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
