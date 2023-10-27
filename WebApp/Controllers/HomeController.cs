using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Npgsql;
using WebApp.Data;
using System.Reflection.Metadata.Ecma335;

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
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Added IActionResult for Login
    public IActionResult Login()
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Added IActionResult for login input
    [HttpPost]
    public IActionResult Login(string username, string password) {
        if (username == "admin" && password == "admin") {
            return RedirectToAction("Index", "Home");
        } else if (username == "user" && password == "user") {
            return RedirectToAction("Privacy", "Home");
        } else {
            return RedirectToAction("Login", "Home");
        }
    }
    
}
