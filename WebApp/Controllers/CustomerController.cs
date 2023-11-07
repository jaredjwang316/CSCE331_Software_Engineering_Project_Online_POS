/*  
    File: CustomerController.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Npgsql;
using WebApp.Data;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace WebApp.Controllers;

public class CustomerController : Controller
{
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ILogger<CustomerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        HttpContext.Session.SetString("Init", "1");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ShowCategories() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<SeriesInfo> unique_series = unit.GetAll<SeriesInfo>().ToList();
        List<string> drink_categories = unit.GetUniqueSeries(true, false, false).ToList();

        string html = "";
        foreach (SeriesInfo series_info in unique_series) {
            if (drink_categories.Contains(series_info.Name)) {
                html += 
                    "<button class=\"menu-item series-btn\" id=\"" + series_info.Name + "\" data-to=\"item-container\">" +
                        "<img src=\"" + series_info.ImgUrl + "\" />" +
                        "<p>" + series_info.Name + "</p>" +
                    "</button>"
                ;
            }
        }

        unit.CloseConnection();
        return Content(html);
    }

    public IActionResult ShowBestSellers() {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<Product> best_selling = unit.GetBestSellingProducts(10).ToList();

        // Generate HTML for each series
        string html = "";
        foreach (Product product in best_selling) {
            html += 
                "<button class=\"menu-item product-btn best-seller\" id=\"" + product.Id + "\" data-to=\"customization-container\">" +
                    "<img src=\"" + product.ImgUrl + "\" />" +
                    "<p>" + product.Name + "</p>" +
                "</button>"
            ;
        }

        unit.CloseConnection();
        return Content(html);
    }

    public IActionResult ShowFavorites() {
        return Content("Not Implemented");
    }

    public IActionResult ShowProductsBySeries(string arg) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<Product> products = unit.GetProductsBySeries(arg).ToList();

        // Generate HTML for each series
        string html = "";
        foreach (Product product in products) {
            html += 
                "<button class=\"menu-item product-btn product\" id=\"" + product.Id + "\" data-to=\"customization-container\">" +
                    "<img src=\"" + product.ImgUrl + "\" />" +
                    "<p>" + product.Name + "</p>" +
                "</button>"
            ;
        }

        unit.CloseConnection();
        return Content(html);
    }

    public IActionResult ShowCustomization(string arg) {
        UnitOfWork unit = new(Config.AWS_DB_NAME);
        List<string> customizations = unit.GetUniqueSeries(false, false, true).ToList();
        List<Product> products = unit.GetAll<Product>().ToList();

        int product_id = Int32.Parse(arg);
        Product selected_product = unit.Get<Product>(product_id);

        //Generate HTML for each series
        string html = "<div class=\"customization-menu\">";
        html += "<div class=\"customization-img-container\"><img src=\"" + selected_product.ImgUrl + "\" />" + selected_product.Name + "</div>";

        foreach (string customization in customizations) {
            html += "<div class=\"customization-title\" id=\"" + customization + "\">" + customization + "</div>";
            html += "<div class=\"customization-btn-container\">";
            foreach (Product product in products) {
                if (product.Series == customization) {
                    html += 
                        "<button class=\"customization-btn product\" id=\"" + product.Id + "\" data-to=\"customization-container\">" +
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

    public Cart GetCurrentCart() {
        string cartKey = "o7rGongChaCart";

        string? cartJson = HttpContext.Session.GetString(cartKey);
        Cart? cart = null;

        if (cartJson == null) {
            cart = new Cart();
            cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(cartKey, cartJson);
        } else {
            cart = JsonConvert.DeserializeObject<Cart>(cartJson);
        }

        return cart!;
    }

    public IActionResult AddToCart(int productID, List<int> customizationIDs) {
        Cart cart = GetCurrentCart();

        UnitOfWork unit = new(Config.AWS_DB_NAME);
        Product product = unit.Get<Product>(productID);
        List<Product> customizations = unit.GetAll<Product>().Where(p => customizationIDs.Contains(p.Id)).ToList();

        CartItem cartItem = new(product, customizations);

        cart.AddItem(cartItem);

        string cartKey = "o7rGongChaCart";
        string cartJson = JsonConvert.SerializeObject(cart);
        HttpContext.Session.SetString(cartKey, cartJson);

        unit.CloseConnection();
        return Ok();
    }




    
    public IActionResult DisplayCart() {
        Cart cart = GetCurrentCart();

        return View(cart);
    }

    [HttpPost]
    public IActionResult ClearCart() {
        Cart cart = GetCurrentCart();
        cart.Clear();

        string cartJson = JsonConvert.SerializeObject(cart);
        HttpContext.Session.SetString("o7rGongChaCart", cartJson);

        return Ok();
    }
}
