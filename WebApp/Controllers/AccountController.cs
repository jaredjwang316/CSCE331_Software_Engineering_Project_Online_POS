using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApp.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly string _returnUrl;

    public AccountController(ILogger<AccountController> logger, string returnUrl)
    {
        _logger = logger;
        _returnUrl = returnUrl;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login(string provider = "Google")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = _returnUrl }, provider);
    }

    [AllowAnonymous]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties() { RedirectUri = _returnUrl }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
