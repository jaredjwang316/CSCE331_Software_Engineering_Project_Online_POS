/*

*/

using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Cart;
using WebApp.Models.UnitOfWork;

namespace WebApp.Controllers;
public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly UnitOfWork unit;

    public CartController(ILogger<CartController> logger, UnitOfWork unit)
    {
        _logger = logger;
        this.unit = unit;
    }

    public IActionResult Index()
    {
        Cart cart = GetCartFromSession();
        return View(cart);
    }

    public IActionResult Clear() {
        // Cart cart = GetCartFromSession();
        // cart.Clear();
        // SetCartInSession(cart);
        // HttpContext.Session.Clear();
        // return Ok();
        throw new NotImplementedException();
    }

    static Product? FindProduct(List<Product> products, int id) {
        return products.FirstOrDefault(p => p.Id == id);
    }

    static List<Product> FindCustomizations(List<Product> products, List<int> ids) {
        return products.Where(p => ids.Contains(p.Id)).ToList();
    }

    public IActionResult AddItem(int product_id, List<int> customization_ids, int quantity) {
        List<Product> products = unit.GetAll<Product>().ToList();

        Product? selected_product = FindProduct(products, product_id);
        List<Product> selected_customizations = FindCustomizations(products, customization_ids);

        if (selected_product == null) return BadRequest();

        Item item = new(selected_product, selected_customizations, quantity);
        Cart cart = GetCartFromSession();
        int initial_size = cart.Items.Sum(i => i.Quantity);

        cart.AddItem(item);
        SetCartInSession(cart);

        int final_size = cart.Items.Sum(i => i.Quantity);

        if (final_size == initial_size) return BadRequest();
        return Ok();

        // UnitOfWork unit = new(Config.AWS_DB_NAME);
        // List<Product> products = unit.GetAll<Product>().ToList();

        // Product? item_product = null;
        // List<Product> item_options = new();
        // foreach (Product product in products) {
        //     if (product_id == product.Id) {
        //         item_product = product;
        //     }
        //     foreach (int customization_id in customization_ids) {
        //         if (customization_id == product.Id) {
        //             item_options.Add(product);
        //             customization_ids.Remove(customization_id);
        //             break;
        //         }
        //     }
        // }
    
        // if (item_product == null) {
        //     return BadRequest();
        // }
        // Item item = new(item_product, item_options, quantity);
        // Cart cart = GetCartFromSession();
        // int initialSize = 0;
        // foreach (Item i in cart.Items) {
        //     initialSize += i.Quantity;
        // }
        // cart.AddItem(item);
        // SetCartInSession(cart);
        // Cart cart1 = GetCartFromSession();
        // int finalSize = 0;
        // foreach (Item i in cart1.Items) {
        //     finalSize += i.Quantity;
        // }

        // if (finalSize == initialSize) {
        //     return BadRequest();
        // }

        // return Ok();
    }

    public IActionResult RemoveItem(int index) {
        // Cart cart = GetCartFromSession();
        // cart.RemoveItem(index);
        // SetCartInSession(cart);
        // return Ok();
        throw new NotImplementedException();
    }

    public IActionResult EditCount(int index, bool isIncrement) {
        Cart cart = GetCartFromSession();
        if (cart.Items[index].Quantity == 1 && !isIncrement) {
            cart.RemoveItem(index);
            SetCartInSession(cart);
            return Ok();
        }
        if (isIncrement) {
            cart.Items[index].Quantity++;
        } else {
            Console.WriteLine(index);
            cart.Items[index].Quantity--;
        }
        SetCartInSession(cart);
        return Ok();
        // throw new NotImplementedException();
    }

    public IActionResult EditOptions(int index) {
        throw new NotImplementedException();
    }

    public Cart GetCartFromSession() {
        if (HttpContext.Session.GetString("Cart") == null) {
            var newCart = new Cart();
            var serializedCart = JsonConvert.SerializeObject(newCart);

            HttpContext.Session.SetString("Cart", serializedCart);
        } else {
            Console.WriteLine("Cart already exists.");

            var cartJson = HttpContext.Session.GetString("Cart");

            return JsonConvert.DeserializeObject<Cart>(cartJson)!;
        }

        // Return the deserialized cart
        return JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart"))!;
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
