/*
    File: AccountController.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Models.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebApp.Data;

namespace WebApp.Controllers;

[AllowAnonymous]
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

    public IActionResult Login(string provider = "Google", string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = Url.Action("LoginCallback", new { returnUrl }) }, provider);
    }

    public async Task<IActionResult> LoginCallback(string returnUrl)
    {
        UnitOfWork unit = new(Config.AWS_DB_NAME);


        var result = await HttpContext.AuthenticateAsync();
        if (result?.Succeeded != true)
        {
            Console.WriteLine("Authentication failed.");
            return RedirectToAction("Login");
        }

        var identity = new ClaimsIdentity(result.Principal!.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

        Claim emailClaim = User.FindFirst(ClaimTypes.Email)!;
        if (emailClaim == null) {
            Console.WriteLine("No email claim found.");
            return Redirect(Config.returnUrl);
        }

        string role = "Customer";
        Employee? employee = unit.GetAll<Employee>().FirstOrDefault(e => e.Email == emailClaim.Value);
        if (employee != null) {
            role = employee.IsManager ? "Manager" : "Cashier";
        }

        identity.AddClaim(new Claim(ClaimTypes.Role, role));

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties() { RedirectUri = Config.returnUrl, IsPersistent = false }
        );

        unit.CloseConnection();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
            return Redirect(returnUrl);
        }
        
        return Redirect(Config.returnUrl);
    }

    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties() { RedirectUri = Config.returnUrl }, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public IActionResult AccessDenied()
    {
        Claim? roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null) {
            Console.WriteLine("Access denied for role: " + roleClaim.Value);
        }
        else {
            Console.WriteLine("Access denied. No role claim found.");
        }
        return Redirect(Config.returnUrl);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public string GetRole() {
        Claim? roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null) {
            return roleClaim.Value;
        }
        return "Customer";
    }

    public string GetName() {
        Claim? nameClaim = User.FindFirst(ClaimTypes.Name);
        if (nameClaim != null) {
            return nameClaim.Value;
        }
        return "Guest";
    }

    public string GetEmail() {
        Claim? emailClaim = User.FindFirst(ClaimTypes.Email);
        if (emailClaim != null) {
            return emailClaim.Value;
        }
        return "";
    }

    public IActionResult GetUserInfo() {
        return Json(new { name = GetName(), role = GetRole(), email = GetEmail() });
    }
}
