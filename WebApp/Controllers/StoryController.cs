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

    [HttpGet, Route("Story/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet, Route("Story/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
