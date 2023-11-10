using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers;

[Authorize(Roles = "Manager")]
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
            var inventory = uok.GetAll<Inventory>().ToList();
            var ingredients = uok.GetAll<Ingredient>().ToList();
            return View((products, inventory, ingredients));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult SaveProducts([FromBody]List<Product> data) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        foreach (Product product in data) {
            Product initial_product = unit.Get<Product>(product.Id);
            product.IsDrink = initial_product.IsDrink;
            product.Hidden = initial_product.Hidden;
            product.IsOption = initial_product.IsOption;
            product.ImgUrl = initial_product.ImgUrl;
            unit.Update<Product>(initial_product, product);
        }

        unit.CloseConnection();

        return Ok();
    }
}
