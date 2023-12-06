using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;

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
            uok.CloseConnection();
            return View((products, inventory, ingredients));
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

    [HttpPost, Route("Manager/SaveInventory")]
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

    [HttpDelete, Route("Manager/ClearCache")]
    public IActionResult ClearCache() {
        cache.Remove("Categories");
        cache.Remove("BestSellers");
        return Ok();
    }
}