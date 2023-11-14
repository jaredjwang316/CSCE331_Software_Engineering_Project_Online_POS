/*

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

    public CartController(ILogger<CartController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        Cart cart = GetCartFromSession();
        return View(cart);
    }

    public IActionResult Clear() {
        Cart cart = GetCartFromSession();
        cart.Clear();
        SetCartInSession(cart);
        HttpContext.Session.Clear();
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
        int initialSize = cart.Items.Count;
        cart.AddItem(item);
        SetCartInSession(cart);
        Cart cart1 = GetCartFromSession();

        if (cart1.Items.Count == initialSize) {
            return BadRequest();
        }

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
            cart.Items[index].Quantity--;
        }
        SetCartInSession(cart);
        return Ok();
    }

    public IActionResult EditOptions(int index) {
        Cart cart = GetCartFromSession();

        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<string> customizations = unit.GetUniqueSeries(false, false, true).ToList();
        List<SeriesInfo> series_info = unit.GetAll<SeriesInfo>().ToList();
        List<Product> products = unit.GetAll<Product>().ToList();

        Product selected_product = cart.Items[index].Product;
        List<Product> selected_options = cart.Items[index].Options.ToList();

        //Generate HTML for each series
        string html = "<div class=\"customization-menu\">";
        html += "<div class=\"customization-img-container\"><img src=\"" + selected_product.ImgUrl + "\" />" + selected_product.Name + "</div>";

        foreach (string customization in customizations) {
            html += "<div class=\"customization-title\" id=\"" + customization + "\">" + customization + "</div>";
            html += "<div class=\"customization-btn-container\">";
            foreach (Product product in products) {
                if (product.Series == customization) {
                    bool multiselect = series_info.Where(x => x.Name == customization).First().MultiSelectable;
                    html += 
                        "<button class=\"customization-btn product\" id=\"" + product.Id + "\" data-to=\"customization-container\" series=" + product.Series + " multiselect=" + multiselect + ">" +
                            "<p>" + product.Name + "</p>" +
                        "</button>"
                    ;
                }
            }
            html += "</div>";
        }

        html += "<button class=\"add-to-cart-btn\" id=\"" + selected_product.Id + "\">Add to Cart</button>";
        html += "</div>";

        unit.CloseConnection();
        return Content(html);
    }

    public Cart GetCartFromSession() {
        if (HttpContext.Session.GetString("Cart") == null) {
            var newCart = new Cart();
            var serializedCart = JsonConvert.SerializeObject(newCart);

            Console.WriteLine("No cart - Serialized cart: " + serializedCart);

            HttpContext.Session.SetString("Cart", serializedCart);
        } else {
            Console.WriteLine("Cart already exists.");

            var cartJson = HttpContext.Session.GetString("Cart");
            Console.WriteLine("Cart JSON: " + cartJson);

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
