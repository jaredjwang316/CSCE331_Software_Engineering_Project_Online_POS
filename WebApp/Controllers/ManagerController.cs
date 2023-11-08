using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Npgsql;
using WebApp.Data;
using System.Reflection.Metadata.Ecma335;

namespace WebApp.Controllers;

public class ManagerController : Controller
{
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(ILogger<ManagerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            var products = uok.GetAll<Product>().ToList();
            var ingredients = uok.GetAll<Inventory>().ToList();
            return View((products, ingredients));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
