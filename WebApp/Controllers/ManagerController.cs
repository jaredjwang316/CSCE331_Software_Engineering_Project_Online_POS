using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;

namespace WebApp.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
    private readonly ILogger<ManagerController> _logger;
    private readonly IMemoryCache cache;
    private readonly CartService cartService;

    public ManagerController(ILogger<ManagerController> logger,  CartService cartService, IMemoryCache cache)
    {
        _logger = logger;
        this.cache = cache;
        this.cartService = cartService;
    }

    public IActionResult Index()
    {

        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        HttpContext.Session.SetString("Init", "1");
      //  return View();
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            List<Product> products= uok.GetAll<Product>()
                .Where(product => product.IsDrink && !product.Hidden && !product.IsOption)
                .ToList();
            var inventory = uok.GetAll<Inventory>().ToList();
            var ingredients = uok.GetAll<Ingredient>().ToList();
            foreach (var ingredient in inventory) {
                if (ingredient.Quantity <= 0) {
                    ingredient.Quantity = ingredient.FillLevel;
                    uok.Update<Inventory>(ingredient, ingredient);
                }
            }
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
            try {
                Product initial_product = unit.Get<Product>(product.Id);
                product.IsDrink = initial_product.IsDrink;
                product.Hidden = initial_product.Hidden;
                product.IsOption = initial_product.IsOption;
                unit.Update<Product>(initial_product, product);
            } catch {
                continue;
            }
            
        }

        unit.CloseConnection();

        ClearCache();

        return Ok();
    }
    public IActionResult AddProduct() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Product product = unit.Get<Product>(-1);
        product.IsDrink = true;
        unit.Add<Product>(product);
        product = unit.GetRecentProduct();
        unit.CloseConnection();

        ClearCache();

        return Ok(product.Id);
    }



    public IActionResult SaveInventory([FromBody] Dictionary<string, string> payload) {
        string data = payload["data"];
        string data2 = payload["data2"];
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        if (!ModelState.IsValid)
        {
            Console.WriteLine("ISNT VALID MODEL BINDING");
            foreach (var entry in ModelState.Values) {
                foreach (var error in entry.Errors)
                {
                    Console.WriteLine($"Property: {entry} Error: {error.ErrorMessage}");
                }
            }
            return BadRequest(ModelState);
        }
        if (data == null) {
            Console.WriteLine("null inventory");
            return BadRequest("Inventory is null");
        }
        if (data2 == null) {
            Console.WriteLine("null ingredient");
            return BadRequest("Ingredient is null");
        }
        List<Inventory>? item1 = JsonConvert.DeserializeObject<List<Inventory>>(data);
        List<Ingredient>? item2 = JsonConvert.DeserializeObject<List<Ingredient>>(data2);

        if (item1 is not null) {
            foreach (Inventory inventory in item1) {
                try {
                    Inventory initial_inventory = unit.Get<Inventory>(inventory.Id);
                    unit.Update<Inventory>(initial_inventory, inventory);
                } catch {
                    continue;
                }
            }
        }

        if (item2 is not null) {
            foreach (Ingredient ingredient in item2) {
                try {
                    Ingredient initial_ingredient = unit.Get<Ingredient>(ingredient.Id);
                    unit.Update<Ingredient>(initial_ingredient, ingredient);
                } catch {
                    continue;
                }
            }
        }

        unit.CloseConnection();

        ClearCache();

        return Ok();
    }

    public IActionResult AddInventory() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        //Inventory inventory = unit.Get<Inventory>(-1);


        unit.CloseConnection();

        ClearCache();
        
        //return Ok(inventory.Id);
        return Ok();
    }

    public IActionResult ClearCache() {
        cache.Remove("Categories");
        cache.Remove("BestSellers");
        return Ok();
    }
}