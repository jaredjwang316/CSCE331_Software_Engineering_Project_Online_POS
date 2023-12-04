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

/// <summary>
/// Controller responsible for handling user account-related actions, such as login, logout, and user preferences.
/// </summary>
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the default view for the account controller.
    /// </summary>
    /// <returns>The default view.</returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Initiates the login process using the specified authentication provider.
    /// </summary>
    /// <param name="provider">The authentication provider (default is "Google").</param>
    /// <param name="returnUrl">The return URL after successful login (default is "/").</param>
    /// <returns>The challenge result for the specified authentication provider.</returns>
    public IActionResult Login(string provider = "Google", string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = Url.Action("LoginCallback", new { returnUrl }) }, provider);
    }

    /// <summary>
    /// Handles the callback after a successful login and sets up user authentication.
    /// </summary>
    /// <param name="returnUrl">The return URL after successful login.</param>
    /// <returns>The result of the login callback.</returns>
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

    /// <summary>
    /// Logs out the user and resets preferences to default.
    /// </summary>
    /// <param name="returnUrl">The return URL after logout.</param>
    /// <returns>The sign-out result.</returns>
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

    /// <summary>
    /// Handles access denied scenarios and redirects to the default URL.
    /// </summary>
    /// <returns>The redirect result after access denied.</returns>
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

    /// <summary>
    /// Displays the error view.
    /// </summary>
    /// <returns>The error view.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Gets the user's role.
    /// </summary>
    /// <returns>The user's role.</returns>
    public string GetRole() {
        Claim? roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null) {
            return roleClaim.Value;
        }
        return "Customer";
    }

    /// <summary>
    /// Gets the user's name.
    /// </summary>
    /// <returns>The user's name.</returns>
    public string GetName() {
        Claim? nameClaim = User.FindFirst(ClaimTypes.Name);
        if (nameClaim != null) {
            return nameClaim.Value;
        }
        return "Guest";
    }

    /// <summary>
    /// Gets the user's email.
    /// </summary>
    /// <returns>The user's email.</returns>
    public string GetEmail() {
        Claim? emailClaim = User.FindFirst(ClaimTypes.Email);
        if (emailClaim != null) {
            return emailClaim.Value;
        }
        return "";
    }

    /// <summary>
    /// Gets the user information in JSON format.
    /// </summary>
    /// <returns>JSON object containing user information.</returns>
    public IActionResult GetUserInfo() {
        return Json(new { name = GetName(), role = GetRole(), email = GetEmail() });
    }

    /// <summary>
    /// Saves user preferences.
    /// </summary>
    /// <param name="accCursor">The accessibility cursor setting.</param>
    /// <param name="accTextSize">The accessibility text size setting.</param>
    /// <param name="accContrast">The accessibility contrast setting.</param>
    /// <param name="accLanguage">The user's preferred language.</param>
    /// <returns>JSON object indicating the success of the operation.</returns>
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

    /// <summary>
    /// Resets user preferences to default settings.
    /// </summary>
    /// <returns>JSON object indicating the success of the operation.</returns>
    public IActionResult ResetPreferencesToDefault() {
        HttpContext.Response.Cookies.Append("Grayscale", "false");
        HttpContext.Response.Cookies.Append("Invert", "false");
        HttpContext.Response.Cookies.Append("BigCursor", "false");
        HttpContext.Response.Cookies.Append("BigText", "false");

        var translateAPI = new GoogleTranslate();
        HttpContext.Response.Cookies.Append("CurrentLanguage", translateAPI.GetPreferredLanguage());
        return Json(new { success = true });
    }
}
