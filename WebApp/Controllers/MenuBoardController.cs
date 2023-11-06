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

    public IActionResult Index()
    {
        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            // Retrieve products and product ingredients from the database
            var products = uok.GetAll<Product>().ToList();
            var prodIngredients = uok.GetAll<ProductIngredients>().ToList();

            // Pass both products and product ingredients to the view
            return View((products, prodIngredients));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ShowCategories()
    {
        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
    
        // Retrieve unique categories from the database
        var categories = uok.GetUniqueSeries().ToList();
    
        // Generate HTML for category buttons
        string html = "";
        foreach (var category in categories)
        {
            html += $"<button class=\"menu-item category-btn\" id=\"{category}\">{category}</button>";
        }
    
        uok.CloseConnection();

        return Content(html);
    }

    public IActionResult ShowProducts()
    {
        // Instantiate the UnitOfWork with the desired configuration
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
    
        // Retrieve products from the database
        var products = uok.GetAll<Product>().ToList();
    
        // Generate HTML for product buttons
        string html = "";
        foreach (var product in products)
        {
            html += $"<button class=\"menu-item product-btn\" id=\"{product.Id}\">{product.Name}</button>";
        }
    
        uok.CloseConnection();

        return Content(html);
    }
}
