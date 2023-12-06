using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[ApiController]
public class StoryController : Controller
{
    private readonly ILogger<StoryController> _logger;

    public StoryController(ILogger<StoryController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Displays the default view for the story controller.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Story/")]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Displays the error view.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Story/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
