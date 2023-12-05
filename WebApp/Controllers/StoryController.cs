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

public class StoryController : Controller
{
    private readonly ILogger<StoryController> _logger;

    public StoryController(ILogger<StoryController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public string GetPreferredLanguage()
    {
        var preferredLanguage = HttpContext.Request.Headers["Accept-Language"].ToString().Split(',')[0];
        return preferredLanguage;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
