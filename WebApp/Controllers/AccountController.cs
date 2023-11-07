/*
    File: AccountController.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

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

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login(string provider = "Google")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = Config.returnUrl }, provider);
    }

    [AllowAnonymous]
    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties() { RedirectUri = Config.returnUrl }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
