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
            var inventory = uok.GetAll<Inventory>().ToList();
            var ingredients = uok.GetAll<Ingredient>().ToList();
            return View((products, inventory, ingredients));
        }
    }

    public IActionResult ShowManager() {
        return Content("Not Implemented");
    }

    public IActionResult ShowProducts() {
        return Content("Not Implemented");
    }

     public IActionResult ShowInventory() {
        return Content("Not Implemented");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // fetch('/Manager/SaveProducts', {
    //         method: 'POST',
    //         headers: {
    //             'Content-Type': 'application/json'
    //         },
    //         body: JSON.stringify(data)
    //     }).then(function(response) {
    //         console.log(response);
    //     }).catch(function(error) {
    //         console.log(error);
    //     });
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

    public IActionResult SaveInventory([FromBody]List<Inventory> data) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        foreach (Inventory inventory in data) {
            Inventory initial_inventory = unit.Get<Inventory>(inventory.Id);
            unit.Update<Inventory>(initial_inventory, inventory);
        }

        unit.CloseConnection();

        return Ok();
    }
}