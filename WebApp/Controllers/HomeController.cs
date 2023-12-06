/*
    File: Controllers/HomeController.cs
    Author: Griffin Beaudreau
    Date: December 1, 2023
    Purpose: The HomeController class manages incoming requests and serves as the entry point for various actions within the web application. 
    This controller handles requests related to user authentication, redirection based on user roles (e.g., Cashier or Manager), and renders 
    views for the application's landing page and privacy information. Additionally, it includes functionalities to determine the user's preferred 
    language based on the 'Accept-Language' header in the HTTP request. Error handling is facilitated through the Error method, returning 
    error-related views with unique identifiers to aid in debugging. The controller also interacts with external services, such as Azure Maps 
    for location-based functionalities and Google Cloud Translation for language-related features. The HomeController leverages the ILogger 
    interface for logging purposes, maintaining system activity and facilitating debugging processes.
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;
/// <summary>
/// Controller responsible for handling requests related to the home page and basic site navigation.
/// </summary>
[ApiController]
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
    [HttpGet, Route("/")]
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
    /// Gets the user's preferred language based on the 'Accept-Language' header in the HTTP request.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("/GetPreferredLanguage")]
    public string GetPreferredLanguage()
    {
        var preferredLanguage = HttpContext.Request.Headers["Accept-Language"].ToString().Split(',')[0];
        return preferredLanguage;
    }

    /// <summary>
    /// Handles errors and displays the error view.
    /// </summary>
    /// <returns>The view for the error page.</returns>
    [HttpGet, Route("/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
