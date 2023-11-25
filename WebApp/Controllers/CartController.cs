/*
    File: Controllers/CartController.cs
    Author: Griffin Beaudreau
    Date: November 24, 2023
*/

using System.Diagnostics;
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
    private readonly CartService cartService;

    public CartController(ILogger<CartController> logger, UnitOfWork unit, CartService cartService)
    {
        _logger = logger;
        this.unit = unit;
        this.cartService = cartService;
    }

    public IActionResult Index()
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

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
        Cart cart = cartService.GetCartFromSession();
        int initial_size = cart.Items.Sum(i => i.Quantity);

        cart.AddItem(item);
        cartService.SetCartInSession(cart);

        int final_size = cart.Items.Sum(i => i.Quantity);

        if (final_size == initial_size) return BadRequest();
        return Ok();
    }

    public IActionResult RemoveItem(int index) {
        Cart cart = cartService.GetCartFromSession();
        cart.RemoveItem(index);
        cartService.SetCartInSession(cart);
        return Ok();
    }

    public IActionResult EditCount(int index, bool isIncrement) {
        Cart cart = cartService.GetCartFromSession();
        if (cart.Items[index].Quantity == 1 && !isIncrement) {
            cart.RemoveItem(index);
            cartService.SetCartInSession(cart);
            return Ok();
        }
        if (isIncrement) {
            cart.Items[index].Quantity++;
        } else {
            cart.Items[index].Quantity--;
        }
        cartService.SetCartInSession(cart);
        return Ok();
    }

    public IActionResult EditOptions(int index) {
        throw new NotImplementedException();
    }

    public IActionResult Checkout(string name, string role, string email) {
        Cart cart = cartService.GetCartFromSession();

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
        cartService.SetCartInSession(cart);

        return Ok();
    }

    public Cart GetCartFromSession() {
        return cartService.GetCartFromSession();
    }


    public void SetCartInSession(Cart cart) {
        cartService.SetCartInSession(cart);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
