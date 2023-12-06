/*
    File: Controllers/AccountController.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
    Purpose: The AccountController class manages user authentication and authorization 
    functionalities within the Point of Sale (POS) system. This controller handles various actions related to user 
    account management, login, logout, access control, and error handling.

    The Login method initiates user authentication using a specified provider (defaulting to Google) and redirects 
    users to the provider's authentication page. Upon successful authentication, the LoginCallback method retrieves 
    user information, assigns appropriate roles (Manager, Cashier, or Customer), and signs in the user, redirecting to 
    the appropriate location based on roles and returnUrl.

    The Logout method handles user sign-out, redirecting users to the appropriate location based on their role. 
    AccessDenied handles scenarios where users encounter unauthorized access, logging details and redirecting to the configured return URL.

    Additionally, the controller includes methods to retrieve user role, name, email, and user information in JSON format.

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
[AllowAnonymous, ApiController]
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
    /// Initiates user authentication using a specified provider (defaulting to Google)
    /// and redirects users to the provider's authentication page.
    /// </summary>
    /// <param name="provider"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet, Route("Account/Login")]
    public IActionResult Login(string provider = "Google", string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties() { RedirectUri = Url.Action("LoginCallback", new { returnUrl }) }, provider);
    }

    /// <summary>
    /// Upon successful authentication, retrieves user information,
    /// assigns appropriate roles (Manager, Cashier, or Customer),
    /// sets user preferences, and signs in the user, redirecting to
    /// the appropriate location based on roles and returnUrl.
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet, Route("Account/LoginCallback")]
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
    /// Handles user sign-out, redirecting users to the appropriate location based on their role.
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet, Route("Account/Logout")]
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
    /// Handles scenarios where users encounter unauthorized access,
    /// logging details and redirecting to the configured return URL.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/AccessDenied")]
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
    /// Handles scenarios where users encounter errors,
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Retrieves user role.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/GetRole")]
    public string GetRole() {
        Claim? roleClaim = User.FindFirst(ClaimTypes.Role);
        if (roleClaim != null) {
            return roleClaim.Value;
        }
        return "Customer";
    }

    /// <summary>
    /// Retrieves user name.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/GetName")]
    public string GetName() {
        Claim? nameClaim = User.FindFirst(ClaimTypes.Name);
        if (nameClaim != null) {
            return nameClaim.Value;
        }
        return "Guest";
    }

    /// <summary>
    /// Retrieves user email.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/GetEmail")]
    public string GetEmail() {
        Claim? emailClaim = User.FindFirst(ClaimTypes.Email);
        if (emailClaim != null) {
            return emailClaim.Value;
        }
        return "";
    }

    /// <summary>
    /// Retrieves user name, role, and email in a JSON format.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Account/GetUserInfo")]
    public IActionResult GetUserInfo() {
        return Json(new { name = GetName(), role = GetRole(), email = GetEmail() });
    }

    /// <summary>
    /// Saves user preferences as cookies and to the database if the user is authenticated.
    /// </summary> 
    /// <param name="accCursor"></param>
    /// <param name="accTextSize"></param>
    /// <param name="accContrast"></param>
    /// <param name="accLanguage"></param>
    /// <returns></returns>
    [HttpPost, Route("Account/SaveUserPreferences")]
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
    /// Resets user preferences to default values.
    /// </summary>
    /// <returns></returns>
    [HttpDelete, Route("Account/ResetPreferencesToDefault")]
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
