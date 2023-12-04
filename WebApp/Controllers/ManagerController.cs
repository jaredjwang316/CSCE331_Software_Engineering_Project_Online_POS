using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling requests related to the manager's view and actions.
/// </summary>
[Authorize(Roles = "Manager")]
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
    /// Displays the manager's view with product, inventory, and ingredient information.
    /// </summary>
    /// <returns>The view for the manager's dashboard.</returns>
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
            uok.CloseConnection();
            return View((products, inventory, ingredients));
        }
    }

    /// <summary>
    /// Displays the manager's information. (Not implemented)
    /// </summary>
    /// <returns>A content result indicating that the feature is not implemented.</returns>
    public IActionResult ShowManager() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Displays the list of products. (Not implemented)
    /// </summary>
    /// <returns>A content result indicating that the feature is not implemented.</returns>
    public IActionResult ShowProducts() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Displays the inventory. (Not implemented)
    /// </summary>
    /// <returns>A content result indicating that the feature is not implemented.</returns>
    public IActionResult ShowInventory() {
        return Content("Not Implemented");
    }

    /// <summary>
    /// Handles errors and displays the error view.
    /// </summary>
    /// <returns>The view for the error page.</returns>
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

    /// <summary>
    /// Saves the updated product information received from the client.
    /// </summary>
    /// <param name="data">The list of products to be updated.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
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

        ClearCache();

        return Ok();
    }


    /// <summary>
    /// Saves the updated inventory information received from the client.
    /// </summary>
    /// <param name="data">The list of inventory items to be updated.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    public IActionResult SaveInventory([FromBody]List<Inventory> data) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        foreach (Inventory inventory in data) {
            Inventory initial_inventory = unit.Get<Inventory>(inventory.Id);
            unit.Update<Inventory>(initial_inventory, inventory);
        }

        unit.CloseConnection();

        ClearCache();

        return Ok();
    }

    /// <summary>
    /// Clears cached data related to categories and best sellers.
    /// </summary>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    public IActionResult ClearCache() {
        cache.Remove("Categories");
        cache.Remove("BestSellers");
        return Ok();
    }
}