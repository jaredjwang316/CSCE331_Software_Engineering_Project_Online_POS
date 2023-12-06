using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;
using Newtonsoft.Json;

namespace WebApp.Controllers;

[Authorize(Roles = "Manager"), ApiController]
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

    [HttpGet, Route("Manager/")]
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
            var productingredients = uok.GetAll<ProductIngredients>().ToList();
            foreach (var ingredient in inventory) {
                if (ingredient.Quantity <= 0) {
                    ingredient.Quantity = ingredient.FillLevel;
                    uok.Update<Inventory>(ingredient, ingredient);
                }
            }
            return View((products, inventory, ingredients, productingredients));
        }
    }

    [HttpGet, Route("Manager/ShowManager")]
    public IActionResult ShowManager() {
        return Content("Not Implemented");
    }

    [HttpGet, Route("Manager/ShowProducts")]
    public IActionResult ShowProducts() {
        return Content("Not Implemented");
    }

    [HttpGet, Route("Manager/ShowInventory")]
    public IActionResult ShowInventory() {
        return Content("Not Implemented");
    }

    [HttpGet, Route("Manager/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost, Route("Manager/SaveProducts")]
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
    
    [HttpPost, Route("Manager/AddProduct")]
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

    [HttpDelete, Route("Manager/DeleteProduct/{prod}")]
    public IActionResult DeleteProduct(int prod) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Product product = unit.Get<Product>(prod);
        unit.Delete(product);
        Console.WriteLine("Can Delete Id: " + product.Id);
        return Ok();
    }

    [HttpPost, Route("Manager/SaveInventory")]
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

    [HttpPost, Route("Manager/AddInventory")]
    public IActionResult AddInventory() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Inventory inventory = unit.Get<Inventory>(-1);
        Ingredient ingredient = unit.Get<Ingredient>(-1);
        unit.Add<Ingredient>(ingredient);
        ingredient = unit.GetRecentIngredient();
        inventory.IngredientId = ingredient.Id;
        unit.Add<Inventory>(inventory);
        inventory = unit.GetRecentInventory();
        

        unit.CloseConnection();

        ClearCache();
        
        return Ok(inventory.Id);
    }

    [HttpDelete, Route("Manager/DeleteInventory/{inv}")]
    public IActionResult DeleteInventory(int inv) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Inventory inventory = unit.Get<Inventory>(inv);
        Ingredient ingredient = unit.Get<Ingredient>(inventory.IngredientId);
        //unit.Delete<Inventory>(inventory);
       // unit.Delete<Ingredient>(ingredient);
        Console.WriteLine("Can Delete Id: " + inventory.Id + " " + ingredient.Id);
        return Ok();
    }

    [HttpPost, Route("Manager/SaveIngredients")]
    public IActionResult ShowSalesReport([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        return Ok();
    }

    [HttpPost, Route("Manager/ShowSalesReport")]
    public IActionResult ShowExcessReport([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        List<Order> orders = unit.GetOrdersBetween(start_time, end_time);
        List<int> ingredient_ids = new();
        List<Ingredient> ingredients = unit.GetAll<Ingredient>().ToList();
        List<ProductIngredients> product_ingredients = unit.GetAll<ProductIngredients>().ToList();
        foreach (Order order in orders) {
            foreach (int id in order.ItemIds) {
                foreach (ProductIngredients product_ingredient in product_ingredients) {
                    if(product_ingredient.ProductId == id) {
                        ingredient_ids = ingredient_ids.Concat(product_ingredient.IngredientIds).ToList();
                    }
                }
            }
        }
        List<Ingredient> excess_ingredients = new();
        var frequency = ingredient_ids.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        foreach (Ingredient ingredient in ingredients) {
            bool found = false;
            foreach (var element in frequency) {
                if(ingredient.Id == element.Key) {
                    if (element.Value <= 10) {
                        excess_ingredients.Add(ingredient);
                        found = true;
                    }
                    else {
                        found = true;
                    }
                }
            }
            if(!found) {
                excess_ingredients.Add(ingredient);
            }
        }

        return Ok(excess_ingredients);
    }

    [HttpPost, Route("Manager/ShowExcessReport")]
    public IActionResult ShowRestockReport([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        return Ok();
    }

    [HttpPost, Route("Manager/ShowRestockReport")]
    public IActionResult ShowSalesTogether([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        return Ok();
    }

    [HttpPost, Route("Manager/ShowPopularityAnalysis")]
    public IActionResult ShowPopularityAnalysis([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        return Ok();
    }

    [HttpDelete, Route("Manager/ClearCache")]
    public IActionResult ClearCache() {
        cache.Remove("Categories");
        cache.Remove("BestSellers");
        return Ok();
    }
}