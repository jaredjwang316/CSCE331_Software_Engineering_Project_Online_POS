using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;

using Google.Cloud.Translation.V2;
using System.Text.Json;

using WebApp.APIs.AzureMaps;
using WebApp.Models.AzureMaps.Weather;
using WebApp.APIs.GoogleTranslate;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling requests related to the home page and basic site navigation.
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger">The logger for HomeController.</param>
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the home page and redirects to specific pages based on user roles.
    /// </summary>
    /// <param name="autoRedirect">Flag indicating whether to automatically redirect users based on their roles.</param>
    /// <returns>The view for the home page or a redirect to a specific page based on user roles.</returns>
    public IActionResult Index(bool autoRedirect = true)
    {
        if (autoRedirect && User.Identity!.IsAuthenticated && User.IsInRole("Cashier")) {
            return RedirectToAction("Index", "Cashier");
        } else if (autoRedirect && User.Identity!.IsAuthenticated && User.IsInRole("Manager")) {
            return RedirectToAction("Index", "Manager");
        }

        return View();
    }

    /// <summary>
    /// Displays the privacy page.
    /// </summary>
    /// <returns>The view for the privacy page.</returns>
    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>
    /// Retrieves the preferred language based on the user's request headers.
    /// </summary>
    /// <returns>The preferred language of the user.</returns>
    public string GetPreferredLanguage()
    {
        var preferredLanguage = HttpContext.Request.Headers["Accept-Language"].ToString().Split(',')[0];
        return preferredLanguage;
    }

    /// <summary>
    /// Handles errors and displays the error view.
    /// </summary>
    /// <returns>The view for the error page.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
