using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Data;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var preferredLanguage = GetPreferredLanguage();
        ViewData["PreferredLanguage"] = preferredLanguage;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult DatabaseExample()
    {   
        UnitOfWork uok = new(Config.AWS_DB_NAME);
        List<Product> products = uok.GetAll<Product>().ToList();
        uok.CloseConnection();
        
        return View(products);
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
