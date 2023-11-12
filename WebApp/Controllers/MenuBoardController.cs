using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Npgsql;
using WebApp.Data;
using System.Reflection.Metadata.Ecma335;

namespace WebApp.Controllers;

public class MenuBoardController : Controller
{
    private readonly ILogger<MenuBoardController> _logger;
    public MenuBoardController(ILogger<MenuBoardController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(string search)
    {
        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);

        // Retrieve products and product ingredients from the database
        var products = uok.GetAll<Product>().ToList();
        var prodIngredients = uok.GetAll<ProductIngredients>().ToList();

        // Filter products based on the search term
        if (!string.IsNullOrEmpty(search))  //if search string is not null nor empty
        {
            search = search.ToLower();  //we need case-insensitive search
            products = products.Where(p => p.Name.ToLower().Contains(search)).ToList(); //filter out product list based on searched term
        }   
        
        // Get unique product categories
        var productCategories = products.Select(p => p.Series).Distinct().ToList();

        // Pass both products and product categories to the view
        return View((products, prodIngredients, productCategories));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

public IActionResult getProducts()
{
    string html = "<div class=\"customization-menu\">";    
    UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
    {
        // Retrieve products and product ingredients from the database
        var products = uok.GetAll<Product>().ToList();
        var prodIngredients = uok.GetAll<ProductIngredients>().ToList();
        List<string> theProducts = uok.GetUniqueSeries(true, true, true).ToList(); 

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

    public IActionResult ProductDetail(int id)
    {
        // Assuming you have a data repository or database context, fetch the product details by ID
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        var product = uok.Get<Product>(id); // Fetch the product by its unique ID

        if (product == null)
        {
            // If the product with the specified ID doesn't exist, you can return a not found response or redirect to an error page
            return NotFound();
        }

        // Pass the product details to the view for rendering
        return View("ProductDetail", product);
    }

}
