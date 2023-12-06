/*
    File: Controllers/CartController.cs
    Author: Griffin Beaudreau
    Date: November 24, 2023
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Cart;
using WebApp.Models.UnitOfWork;
using WebApp.Models.JsonModels;

namespace WebApp.Controllers;

/* The `CartController.cs` manages the functionality associated with the shopping cart within the web application. 
Primarily responsible for handling cart interactions, this controller orchestrates operations such as adding, 
removing, or editing items, alongside facilitating the checkout process. Within its methods, it manipulates 
cart items stored in the session, performs operations including adding items with customizations, 
modifying quantities, and removing items. Additionally, it interfaces with the unit of work to retrieve 
product information and series details for editing cart items. Notably, this controller also supports user 
checkout by generating orders based on the current cart contents and employee/customer information. 
By implementing these functionalities, the controller efficiently organizes cart-related actions and 
integrates them seamlessly into the web application's shopping experience */

[ApiController]
public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly CartService cartService;

    public CartController(ILogger<CartController> logger, CartService cartService)
    {
        _logger = logger;
        this.cartService = cartService;
    }

    [HttpGet, Route("/Cart")]
    public IActionResult Index()
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        return View(cart);
    }

    [HttpGet, Route("/Cart/FindProduct")]
    static Product? FindProduct(List<Product> products, int id) {
        return products.FirstOrDefault(p => p.Id == id);
    }

    [HttpGet, Route("/Cart/FindCustomizations")]
    static List<Product> FindCustomizations(List<Product> products, List<int> ids) {
        return products.Where(p => ids.Contains(p.Id)).ToList();
    }

    [HttpPost, Route("/Cart/AddItem")]
    public IActionResult AddItem([FromBody] AddItemModel model) {
        UnitOfWork unit = new();
        List<Product> products = unit.GetAll<Product>().ToList();
        unit.CloseConnection();

        Product? selected_product = FindProduct(products, model.ProductId);
        List<Product> selected_customizations = FindCustomizations(products, model.CustomizationIds);

        if (selected_product == null) return BadRequest();

        Item item = new(selected_product, selected_customizations, model.Quantity);
        Cart cart = cartService.GetCartFromSession();
        int initial_size = cart.Items.Sum(i => i.Quantity);

        cart.AddItem(item);
        cartService.SetCartInSession(cart);

        int final_size = cart.Items.Sum(i => i.Quantity);

        if (final_size == initial_size) return BadRequest();
        return Ok();
    }

    [HttpPost, Route("/Cart/RemoveItem")]
    public IActionResult RemoveItem(int index) {
        Cart cart = cartService.GetCartFromSession();
        cart.RemoveItem(index);
        cartService.SetCartInSession(cart);
        return Ok();
    }

    [HttpPost, Route("/Cart/EditCount")]
    public IActionResult EditCount([FromBody] EditCountModel model) {
        Cart cart = cartService.GetCartFromSession();
        if (cart.Items[model.Index].Quantity == 1 && !model.IsIncrement) {
            cart.RemoveItem(model.Index);
            cartService.SetCartInSession(cart);
            return Ok();
        }
        if (model.IsIncrement) {
            cart.Items[model.Index].Quantity++;
        } else {
            cart.Items[model.Index].Quantity--;
        }
        cartService.SetCartInSession(cart);
        return Ok();
    }

    [HttpPost, Route("/Cart/Checkout")]
    public IActionResult Checkout([FromBody] CheckoutModel model) {
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
        UnitOfWork unit = new();
        if (model.Role != "Customer") {
            employee_id = unit.GetAll<Employee>().FirstOrDefault(e => e.Email == model.Email)!.Id;
        }

        Order order = new(-1, employee_id, model.Name, DateTime.Now, cart.TotalCost(), ids);
        unit.Add(order);
        unit.CloseConnection();

        cart.Clear();
        cartService.SetCartInSession(cart);

        return Ok();
    }

    [HttpGet, Route("/Cart/GetCartFromSession")]
    public Cart GetCartFromSession() {
        return cartService.GetCartFromSession();
    }

    [HttpPost, Route("/Cart/SetCartInSession")]
    public void SetCartInSession(Cart cart) {
        cartService.SetCartInSession(cart);
    }

    [HttpGet, Route("/Cart/GetCartSize")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
