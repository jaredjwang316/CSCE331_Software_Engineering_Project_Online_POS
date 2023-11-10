/*

*/

using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Cart;

namespace WebApp.Controllers;
public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;

    public CartController(ILogger<CartController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Clear() {
        Cart cart = GetCartFromSession();
        Cart.Clear();
        SetCartInSession(cart);
        return Ok();
    }

    public IActionResult AddItem(int product_id, List<int> customization_ids, int quantity) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<Product> products = unit.GetAll<Product>().ToList();

        Product? item_product = null;
        List<Product> item_options = new();
        foreach (Product product in products) {
            if (product_id == product.Id) {
                item_product = product;
            }
            foreach (int customization_id in customization_ids) {
                if (customization_id == product.Id) {
                    item_options.Add(product);
                    customization_ids.Remove(customization_id);
                    break;
                }
            }
        }

        if (item_product == null) {
            return BadRequest();
        }
        Item item = new(item_product, item_options, quantity);
        Cart cart = GetCartFromSession();
        Cart.AddItem(item);
        SetCartInSession(cart);

        return Ok();
    }

    public Cart GetCartFromSession() {
        var cartJson = HttpContext.Session.GetString("Cart");
        return cartJson == null ? new() : JsonConvert.DeserializeObject<Cart>(cartJson)!;
    }

    public void SetCartInSession(Cart cart) {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
