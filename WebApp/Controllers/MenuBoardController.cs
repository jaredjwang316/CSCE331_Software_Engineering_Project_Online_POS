using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Models.Cart;
using WebApp.Data;

namespace WebApp.Controllers;
/* This part of the program manages the display and functionality of the menu board in our Point of Sale (POS) system.
The MenuBoardController orchestrates the retrieval of products, product categories, and their details from the database. 
The 'Index' method generates the main menu board view, allowing users to search for specific items, 
displaying them by categories, and handling various interactions. 
Additionally, 'getProducts' retrieves and organizes items and categories for the frontend, enabling the creation of the 
interactive menu interface. 
This controller also handles errors through the 'Error' method and provides detailed 
product information through the 'ProductDetail' method. Overall, this controller acts as the bridge between the 
database and the user interface, facilitating the effective presentation and interaction with menu items within the 
POS system. */

[ApiController]
public class MenuBoardController : Controller
{
    private readonly ILogger<MenuBoardController> _logger;
    private readonly CartService cartService;

    public MenuBoardController(ILogger<MenuBoardController> logger, CartService cartService)
    {
        _logger = logger;
        this.cartService = cartService;
    }
    
    [HttpGet, Route("MenuBoard/")]
    public IActionResult Index(string? search = null)
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new(Config.AWS_DB_NAME);

        // Retrieve products and product ingredients from the database
        var products = uok.GetAll<Product>().ToList();
        var prodIngredients = uok.GetAll<ProductIngredients>().ToList();

        uok.CloseConnection();

        // Filter products based on the search term
        if (!string.IsNullOrEmpty(search))  //if search string is not null nor empty
        {
            search = search.ToLower();  //we need case-insensitive search
            products = products.Where(p => p.Name.ToLower().Contains(search)).ToList(); //filter out product list based on searched term
        }   
        
        // Get unique product categories
        var productCategories = products.Select(p => p.Series).Distinct().ToList();

        Dictionary<string, List<Product>> categoryProducts = new Dictionary<string, List<Product>>();
    
        foreach (var category in productCategories)
        {
            var categoryProductsList = products.Where(p => p.Series == category).ToList();
            categoryProducts.Add(category, categoryProductsList);
        }
    
        ViewBag.CategoryProducts = categoryProducts;

        // Pass both products and product categories to the view
        return View((products, prodIngredients, productCategories));
    }

    [HttpGet, Route("MenuBoard/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {   
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet, Route("MenuBoard/getProducts")]
    public IActionResult getProducts()
    {
        string html = "<div class=\"customization-menu\">";    
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            // Retrieve products and product ingredients from the database
            var products = uok.GetAll<Product>().ToList();
            // var prodIngredients = uok.GetAll<ProductIngredients>().ToList();
            List<string> theProducts = uok.GetUniqueSeries(true, false, false).ToList(); 
            uok.CloseConnection();

            foreach (var p in theProducts)
            {
                html += "<div class=\"product-category\">";
                html += "<button class=\"customization-btn category\" data-category=\"" + p + "\">" + p + "</button>";

                html += "<div class=\"product-list\">";
                foreach (var product in products)
                {
                    if (product.Series == p)    //check if current product belongs to that series
                    {
                        html += "<button class=\"customization-btn product\" id=\"" + product.Id + "\" data-to=\"customization-container\">" +
                        "<p>" + product.Name + "</p>" +
                        "<p>ID: " + product.Id + "</p>" +
                        "</button>";
                    }
                }
                html += "</div>"; // Close product-list div
                html += "</div>"; // Close product-category div
            }
            html += "</div>"; // Close customization-menu div
        }        
        return Content(html);   //return the string html
    }

    [HttpGet, Route("MenuBoard/GetProductsByCategory")]
    public IActionResult GetProductsByCategory(string category)
    {
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        var products = uok.GetAll<Product>().Where(p => p.Series == category).ToList(); 
        var productNames = products.Select(p => p.Name).ToList();

        uok.CloseConnection();

        return PartialView("_ProductNamesPartial", productNames);
    }

    [HttpGet, Route("MenuBoard/ProductDetail")]
    public IActionResult ProductDetail(int id)
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        // Assuming you have a data repository or database context, fetch the product details by ID
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        var product = uok.Get<Product>(id); // Fetch the product by its unique ID
        uok.CloseConnection();

        if (product == null)
        {
            // If the product with the specified ID doesn't exist, you can return a not found response or redirect to an error page
            return NotFound();
        }

        // Pass the product details to the view for rendering
        return View("ProductDetail", product);
    }

}
