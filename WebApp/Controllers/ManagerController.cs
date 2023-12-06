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

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagerController"/> class.
    /// </summary>
    /// <param name="logger">The logger for ManagerController.</param>
    /// <param name="cartService">The service for managing the shopping cart.</param>
    /// <param name="cache">The cache for storing and retrieving data.</param>
    public ManagerController(ILogger<ManagerController> logger,  CartService cartService, IMemoryCache cache)
    {
        _logger = logger;
        this.cache = cache;
        this.cartService = cartService;
    }

    /// <summary>
    /// Displays the default view for the manager controller.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Manager/ShowManager")]
    public IActionResult ShowManager() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Manager/ShowProducts")]
    public IActionResult ShowProducts() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Manager/ShowInventory")]
    public IActionResult ShowInventory() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Displays the error view.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("Manager/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Saves a list of products changes to the database.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// Adds a new product to the database.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Deletes a product from the database.
    /// </summary>
    /// <param name="prod"></param>
    /// <returns></returns>
    [HttpDelete, Route("Manager/DeleteProduct/{prod}")]
    public IActionResult DeleteProduct(int prod) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Product product = unit.Get<Product>(prod);
        ProductIngredients pi = unit.Get<ProductIngredients>(prod);
        unit.Delete<Product>(product);
        unit.Delete<ProductIngredients>(pi);
        Console.WriteLine("Can Delete Id: " + product.Id);
        unit.CloseConnection();
        return Ok();
    }

    [HttpGet, Route("Manager/GetProductIngredients/{pid}")]
    public IActionResult GetProductIngredients(int pid) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        ProductIngredients pi = unit.Get<ProductIngredients>(pid);
        if (pid == -1) {
            Console.WriteLine("new product");
            return Ok();
        }
        List<int> ids = pi.IngredientIds;
        List<Ingredient> ingredients = unit.GetAll<Ingredient>().ToList();
        List<string> output = new();
        foreach (int id in ids) {
            foreach (Ingredient ingredient in ingredients) {
                if(ingredient.Id == id) {
                    output.Add(ingredient.Name);
                }
            }
        }

        unit.CloseConnection();
        return Ok(output);
    }

    [HttpPost, Route("Manager/EditProductIngredients")]
    public IActionResult EditProductIngredients([FromBody] List<string> ings) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        int count = 0;
        int pid = 0;
        List<int> iids = new();
        List<Ingredient> ingredients = unit.GetAll<Ingredient>().ToList();
        foreach (string ing in ings) {
            if(count == 0) {
                pid = Int32.Parse(ing);
            }
            else {
                foreach (Ingredient ingredient in ingredients) {
                    if (ingredient.Name == ing) {
                        iids.Add(ingredient.Id);
                    }
                }
            }
            count++;
        }

        ProductIngredients newpi = new(pid, iids);
        ProductIngredients initialpi = unit.Get<ProductIngredients>(newpi.ProductId);
        if(initialpi.ProductId == -1) {
            unit.Add<ProductIngredients>(newpi);
        }
        else {
            unit.Update<ProductIngredients>(initialpi, newpi);
        }
        unit.CloseConnection();
        return Ok();
    }


    /// <summary>
    /// Saves a list of inventory changes to the database.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost, Route("Manager/SaveInventory")]
    public IActionResult SaveInventory([FromBody] Dictionary<string, string> payload) {
        string data = payload["data"];
        string data2 = payload["data2"];
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        // USE TO REPLACE PRODUCT INGREDIENTS IF AN INGREDIENT IS DELETED
        // List<ProductIngredients> pi = unit.GetAll<ProductIngredients>().ToList();
        // foreach (var p in pi) {
        //     List<int> ids = new();
        //     foreach (var i in p.IngredientIds) {
        //         if (i == 1061) {
        //             ids.Add(1070);
        //         }
        //         else {
        //             ids.Add(i);
        //         }
        //     }
        //     ProductIngredients p2 = p;
        //     p2.IngredientIds = ids;
        //     unit.Update<ProductIngredients>(p, p2);
        // }
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

    /// <summary>
    /// Adds a new inventory item to the database.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Deletes an inventory item from the database.
    /// </summary>
    /// <param name="inv"></param>
    /// <returns></returns>
    [HttpDelete, Route("Manager/DeleteInventory/{inv}")]
    public IActionResult DeleteInventory(int inv) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Inventory inventory = unit.Get<Inventory>(inv);
        Ingredient ingredient = unit.Get<Ingredient>(inventory.IngredientId);
        unit.Delete<Inventory>(inventory);
        unit.Delete<Ingredient>(ingredient);
        Console.WriteLine("Can Delete Id: " + inventory.Id + " " + ingredient.Id);
        unit.CloseConnection();
        return Ok();
    }

    /// <summary>
    /// Clears the cache for categories and best sellers.
    /// </summary>
    /// <returns></returns>
    [HttpDelete, Route("Manager/ClearCache")]
    public IActionResult ClearCache() {
        cache.Remove("Categories");
        cache.Remove("BestSellers");
        return Ok();
    }

    /// <summary>
    /// Displays the sales report.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost, Route("Manager/ShowSalesReport")]
    public IActionResult ShowSalesReport([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        List<Tuple<int,double>> input = unit.GetSalesReport(start_time, end_time);
        List<Product> products = unit.GetAll<Product>().ToList();
        List<Tuple<string, double>> output = new();
        foreach (var outp in input) {
            foreach (Product product in products) {
                if (product.Id == outp.Item1) {
                    output.Add(new Tuple<string, double>(product.Name, outp.Item2));
                }
            }
        }
        unit.CloseConnection();
        return Ok(output);
    }

    /// <summary>
    /// Displays the excess report.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost, Route("Manager/ShowExcessReport")]
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
        unit.CloseConnection();
        return Ok(excess_ingredients);
    }

    /// <summary>
    /// Displays the restock report.
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("Manager/ShowRestockReport")]
    public IActionResult ShowRestockReport() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<Inventory> inventory = unit.GetAll<Inventory>().ToList();
        List<Ingredient> ingredients = unit.GetAll<Ingredient>().ToList();
        List<Ingredient> output = new();
        foreach (Inventory ing in inventory) {
            if (ing.Quantity < 0.2*ing.FillLevel) {
                foreach (Ingredient ingredient in ingredients) {
                    if (ingredient.Id == ing.IngredientId) {
                        output.Add(ingredient);
                    }
                }
            }
        }
        unit.CloseConnection();
        return Ok(output);
    }

    /// <summary>
    /// Displays the sales together report.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost, Route("Manager/ShowSalesTogether")]
    public IActionResult ShowSalesTogether([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        List<Tuple<string,string,int>> output = unit.GetSalesTogether(start_time, end_time);
        unit.CloseConnection();
        return Ok(output);
    }

    /// <summary>
    /// Displays the popularity analysis report.
    /// </summary>
    /// <param name="payload"></param>
    /// <returns></returns>
    [HttpPost, Route("Manager/ShowPopularityAnalysis")]
    public IActionResult ShowPopularityAnalysis([FromBody] Dictionary<string, string> payload) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        string data = payload["data"];
        string data2 = payload["data2"];
        DateTime start_time = JsonConvert.DeserializeObject<DateTime>(data);
        DateTime end_time = JsonConvert.DeserializeObject<DateTime>(data2);
        Console.WriteLine(start_time);
        Console.WriteLine(end_time);
        unit.CloseConnection();
        return Ok();
    }
}