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

    public IActionResult getProducts()
    {
        string html = "<div class=\"customization-menu\">";    
        UnitOfWork uok = new UnitOfWork(Config.AWS_DB_NAME);
        {
            // Retrieve products and product ingredients from the database
            var products = uok.GetAll<Product>().ToList();
            var prodIngredients = uok.GetAll<ProductIngredients>().ToList();
            List<string> theDrinks = uok.GetUniqueSeries(true, false, false).ToList(); //gets the series that belong to drinks

            foreach (var drink in theDrinks)
            {
                foreach (var product in products)
                {
                    if (product.Series == drink)    //check if current product belongs to that series
                    {
                        html += "<button class=\"customization-btn product\" id=\"" + product.Id + "\" data-to=\"customization-container\">" +
                            "<p>" + product.Name + "</p>" +
                        "</button>" ;                      
                    }
                }
            }
            html += "</div";
        }        
        return Content(html);   //return the string html
    }
    
}
