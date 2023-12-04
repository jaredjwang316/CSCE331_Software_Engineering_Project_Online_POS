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
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;
/// <summary>
/// Controller responsible for managing shopping cart operations, including adding, removing, and editing items.
/// </summary>
public class CartController : Controller
{
    private readonly ILogger<CartController> _logger;
    private readonly CartService cartService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cartService">The service responsible for cart operations.</param>
    public CartController(ILogger<CartController> logger, CartService cartService)
    {
        _logger = logger;
        this.cartService = cartService;
    }

    /// <summary>
    /// Displays the shopping cart contents along with the total number of items.
    /// </summary>
    /// <returns>The view containing the shopping cart information.</returns>
    public IActionResult Index()
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        return View(cart);
    }

    static Product? FindProduct(List<Product> products, int id) {
        return products.FirstOrDefault(p => p.Id == id);
    }

    static List<Product> FindCustomizations(List<Product> products, List<int> ids) {
        return products.Where(p => ids.Contains(p.Id)).ToList();
    }

    /// <summary>
    /// Adds an item to the shopping cart based on the provided product and customizations.
    /// </summary>
    /// <param name="product_id">The ID of the product to add.</param>
    /// <param name="customization_ids">The IDs of customizations to add.</param>
    /// <param name="quantity">The quantity of the item to add.</param>
    /// <returns>An HTTP result indicating the success of the operation.</returns>
    public IActionResult AddItem(int product_id, List<int> customization_ids, int quantity) {
        UnitOfWork unit = new();
        List<Product> products = unit.GetAll<Product>().ToList();
        unit.CloseConnection();

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

    /// <summary>
    /// Removes an item from the shopping cart at the specified index.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    /// <returns>An HTTP result indicating the success of the operation.</returns>
    public IActionResult RemoveItem(int index) {
        Cart cart = cartService.GetCartFromSession();
        cart.RemoveItem(index);
        cartService.SetCartInSession(cart);
        return Ok();
    }

    /// <summary>
    /// Edits the quantity of an item in the shopping cart.
    /// </summary>
    /// <param name="index">The index of the item to edit.</param>
    /// <param name="isIncrement">A flag indicating whether to increment or decrement the quantity.</param>
    /// <returns>An HTTP result indicating the success of the operation.</returns>
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

    /// <summary>
    /// Displays the partial view for editing item options.
    /// </summary>
    /// <param name="index">The index of the item to edit.</param>
    /// <returns>The partial view for editing item options.</returns>
    public IActionResult EditOptions(int index) {
        UnitOfWork unit = new();
        List<SeriesInfo> seriesInformation = unit.GetAll<SeriesInfo>()
            .Where(series => !series.IsHidden && series.IsCustomization)
            .ToList();
        List<Product> products = unit.GetAll<Product>()
            .Where(product => seriesInformation.Any(series => series.Name == product.Series))
            .ToList();
        
        unit.CloseConnection();

        var model = new EditViewModel {
            Cart = cartService.GetCartFromSession(),
            Index = index,
            SeriesInformation = seriesInformation,
            Products = products
        };

        return PartialView("_EditPartial", model);
    }

    /// <summary>
    /// Processes the checkout operation, creating an order based on the items in the shopping cart.
    /// </summary>
    /// <param name="name">The name associated with the order.</param>
    /// <param name="role">The role of the user performing the checkout.</param>
    /// <param name="email">The email of the user performing the checkout.</param>
    /// <returns>An HTTP result indicating the success of the checkout operation.</returns>
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
        UnitOfWork unit = new();
        if (role != "Customer") {
            employee_id = unit.GetAll<Employee>().FirstOrDefault(e => e.Email == email)!.Id;
        }

        Order order = new(-1, employee_id, name, DateTime.Now, cart.TotalCost(), ids);
        unit.Add(order);
        unit.CloseConnection();

        cart.Clear();
        cartService.SetCartInSession(cart);

        return Ok();
    }

    /// <summary>
    /// Retrieves the shopping cart from the session.
    /// </summary>
    /// <returns>The current shopping cart.</returns>
    public Cart GetCartFromSession() {
        return cartService.GetCartFromSession();
    }

    /// <summary>
    /// Sets the provided shopping cart in the session.
    /// </summary>
    /// <param name="cart">The shopping cart to set in the session.</param>
    public void SetCartInSession(Cart cart) {
        cartService.SetCartInSession(cart);
    }

    /// <summary>
    /// Displays the error view.
    /// </summary>
    /// <returns>The error view.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
