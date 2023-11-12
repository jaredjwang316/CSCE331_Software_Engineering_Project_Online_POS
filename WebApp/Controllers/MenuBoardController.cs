using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;

namespace WebApp.Controllers;

public class MenuBoardController : Controller
{
    private readonly ILogger<MenuBoardController> _logger;
    public MenuBoardController(ILogger<MenuBoardController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            // Retrieve products and product ingredients from the database
            var products = uok.GetAll<Product>().ToList();
            var prodIngredients = uok.GetAll<ProductIngredients>().ToList();

            // Pass both products and product ingredients to the view
            return View((products, prodIngredients));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
