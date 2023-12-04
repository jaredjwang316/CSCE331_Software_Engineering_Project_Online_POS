using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models.ViewModels;
using WebApp.Data;
using Microsoft.Extensions.Caching.Memory;


namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for managing cashier-related operations, including product categories, best sellers, favorites, and customizations.
/// </summary>
[Authorize(Roles = "Cashier, Manager")]
public class CashierController : Controller
{
    private readonly ILogger<CashierController> _logger;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CashierController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    public CashierController(ILogger<CashierController> logger, IMemoryCache cache)
    {
        _logger = logger;
        this.cache = cache;
    }
    /// <summary>
    /// Displays the default view for the cashier controller.
    /// </summary>
    /// <returns>The default view.</returns>
    public IActionResult Index()
    {
        return View();
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

    /// <summary>
    /// Loads and returns the product categories from the cache or database.
    /// </summary>
    /// <returns>The partial view containing the loaded product categories.</returns>
    public IActionResult LoadCategories() {
        UnitOfWork unit = new();
        List<SeriesInfo> model = cache.GetOrCreate("Categories", entry => {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return unit.GetAll<SeriesInfo>()
            .Where(series => !series.IsHidden && series.IsProduct)
            .ToList();
        })!;

        unit.CloseConnection();
        
        return PartialView("_CategoriesPartial", model);
    }

    /// <summary>
    /// Loads and returns the best-selling products from the cache or database.
    /// </summary>
    /// <returns>The partial view containing the loaded best-selling products.</returns>
    public IActionResult LoadBestSellers() {
        UnitOfWork unit = new();
        List<Product> model = cache.GetOrCreate("BestSellers", entry => {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return unit.GetBestSellingProducts(10).ToList();
        })!;

        unit.CloseConnection();

        return PartialView("_ProductsPartial", model);
    }

    /// <summary>
    /// Placeholder method for loading favorites (not yet implemented).
    /// </summary>
    /// <returns>A content result indicating that the feature is not implemented.</returns>
    public IActionResult LoadFavorites() {
        // Use _ProductsPartial when favorites are implemented
        return Content("Not Implemented");
    }

    /// <summary>
    /// Loads and returns products based on the specified series name.
    /// </summary>
    /// <param name="arg">The series name.</param>
    /// <returns>The partial view containing the loaded products.</returns>
    public IActionResult LoadProductsBySeries(string arg) {
        UnitOfWork unit = new();
        List<Product> model = unit.GetProductsBySeries(arg).ToList();
        unit.CloseConnection();
        return PartialView("_ProductsPartial", model);
    }

    /// <summary>
    /// Loads and returns customizations for a selected product.
    /// </summary>
    /// <param name="arg">The ID of the selected product.</param>
    /// <returns>The partial view containing the loaded customizations.</returns>
    public IActionResult LoadCustomizations(string arg) {
        UnitOfWork unit = new();
        Product selectedProduct = unit.Get<Product>(int.Parse(arg));
        List<SeriesInfo> seriesInformation = unit.GetAll<SeriesInfo>()
            .Where(series => !series.IsHidden && series.IsCustomization)
            .ToList();
        List<Product> products = unit.GetAll<Product>()
            .Where(product => seriesInformation.Any(series => series.Name == product.Series))
            .ToList();
        CustomizationViewModel model = new() {
            SelectedProduct = selectedProduct,      // The selected product
            SeriesInformation = seriesInformation,  // Information about each customization series
            Products = products                     // All products that are customizations
        };

        unit.CloseConnection();
        
        return PartialView("_CustomizationsPartial", model);
    }
}
