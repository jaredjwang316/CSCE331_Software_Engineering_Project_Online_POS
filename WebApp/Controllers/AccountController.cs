/*
    File: Controllers/AccountController.cs
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
using WebApp.APIs.GoogleTranslate;

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
        UnitOfWork unit = new();

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

        string name = result.Principal!.FindFirst(ClaimTypes.Name)!.Value ?? "";
        string email = result.Principal!.FindFirst(ClaimTypes.Email)!.Value ?? "";
        User? user = unit.GetUser(email);
        if (user == null) {
            var googleTranslateAPI = new GoogleTranslate();
            string accLanguage = googleTranslateAPI.GetPreferredLanguage();
            unit.Add(new User(
                name,
                email,
                Array.Empty<int>(),
                accLanguage: accLanguage
            ));
        } else {
            HttpContext.Response.Cookies.Append("CurrentLanguage", user.AccLanguage);
            var grayscale = user.AccContrast == "Grayscale";
            var invert = user.AccContrast == "Invert";
            HttpContext.Response.Cookies.Append("Grayscale", grayscale.ToString().ToLower());
            HttpContext.Response.Cookies.Append("Invert", invert.ToString().ToLower());
            HttpContext.Response.Cookies.Append("BigCursor", user.AccCursor.ToString().ToLower());
            HttpContext.Response.Cookies.Append("BigText", user.AccTextSize.ToString().ToLower());
        }
        
        unit.CloseConnection();

        bool isManagerOrCashier = role == "Manager" || role == "Cashier";
        if (!isManagerOrCashier && !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) {
            return Redirect(returnUrl);
        }
        
        return Redirect(Config.returnUrl);
    }

    public IActionResult Logout(string returnUrl)
    {
        bool isManagerOrCashier = GetRole() == "Manager" || GetRole() == "Cashier";
        returnUrl ??= Config.returnUrl;
        if (isManagerOrCashier) {
            returnUrl = Config.returnUrl;
        }
        ResetPreferencesToDefault();
        return SignOut(new AuthenticationProperties() { RedirectUri = returnUrl }, CookieAuthenticationDefaults.AuthenticationScheme);
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

    public IActionResult SaveUserPreferences(
        string? accCursor = null,
        string? accTextSize = null,
        string? accContrast = null,
        string? accLanguage = null)
    {
        if (User.Identity?.IsAuthenticated != true) {
            return Json(new { success = false });
        }

        UnitOfWork unit = new();
        string email = GetEmail();
        User? user = unit.GetUser(email);
        User? newUser = user;
        if (user == null || newUser == null) {
            return Json(new { success = false });
        }
        newUser.AccCursor = accCursor ?? user.AccCursor;
        newUser.AccTextSize = accTextSize ?? user.AccTextSize;
        newUser.AccContrast = accContrast ?? user.AccContrast;
        newUser.AccLanguage = accLanguage ?? user.AccLanguage;
        unit.Update(user, newUser);
        unit.CloseConnection();
        return Json(new { success = true });
    }

    public IActionResult ResetPreferencesToDefault() {
        HttpContext.Response.Cookies.Append("Grayscale", "false");
        HttpContext.Response.Cookies.Append("Invert", "false");
        HttpContext.Response.Cookies.Append("BigCursor", "false");
        HttpContext.Response.Cookies.Append("BigText", "false");
        HttpContext.Response.Cookies.Append("CurrentLanguage", "en");
        return Json(new { success = true });
    }
}
