/*

*/

using System;
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
    }

    public IActionResult RemoveItem(int index) {
        Cart cart = GetCartFromSession();
        cart.RemoveItem(index);
        SetCartInSession(cart);
        return Ok();
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
    }

    public IActionResult EditOptions(int index) {
        throw new NotImplementedException();
    }

    public IActionResult Checkout(string name, string role, string email) {
        Cart cart = GetCartFromSession();

        if (cart == null) return BadRequest();

        List<int> ids = new();
        foreach (Item item in cart.Items) {
            for (int i = 0; i < item.Quantity; i++) {
                ids.Add(item.Product.Id);
                foreach (Product product in item.Options) {
                    ids.Add(product.Id);
                }
            }
        }

        int employee_id = 0;
        if (role != "Customer") {
            employee_id = unit.GetAll<Employee>().FirstOrDefault(e => e.Email == email)!.Id;
        }

        Order order = new(-1, employee_id, name, DateTime.Now, cart.TotalCost(), ids);
        unit.Add(order);

        cart.Clear();
        SetCartInSession(cart);

        return Ok();
    }

    public Cart GetCartFromSession() {
        if (HttpContext.Session.GetString("Cart") == null) {
            var newCart = new Cart();
            var serializedCart = JsonConvert.SerializeObject(newCart);

            HttpContext.Session.SetString("Cart", serializedCart);
        } else {
            Console.WriteLine("Cart already exists.");

            var cartJson = HttpContext.Session.GetString("Cart") ?? throw new Exception("Cart is null");
            return JsonConvert.DeserializeObject<Cart>(cartJson)!;
        }

        // Return the deserialized cart
        return JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart")!)!;
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
