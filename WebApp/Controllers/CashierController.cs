using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models.ViewModels;
using WebApp.Data;
using Microsoft.Extensions.Caching.Memory;


namespace WebApp.Controllers;

[Authorize(Roles = "Cashier, Manager")]
public class CashierController : Controller
{
    private readonly ILogger<CashierController> _logger;
    private readonly UnitOfWork unit;
    private readonly IMemoryCache cache;

    public CashierController(ILogger<CashierController> logger, UnitOfWork unit, IMemoryCache cache)
    {
        _logger = logger;
        this.unit = unit;
        this.cache = cache;
    }
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult LoadCategories() {
        List<SeriesInfo> model = cache.GetOrCreate("Categories", entry => {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            return unit.GetAll<SeriesInfo>()
            .Where(series => !series.IsHidden && series.IsProduct)
            .ToList();
        })!;
        
        return PartialView("_CategoriesPartial", model);
    }

    public IActionResult LoadBestSellers() {
        List<Product> model = cache.GetOrCreate("BestSellers", entry => {
            entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            return unit.GetBestSellingProducts(10).ToList();
        })!;

        return PartialView("_ProductsPartial", model);
    }

    public IActionResult LoadFavorites() {
        // Use _ProductsPartial when favorites are implemented
        return Content("Not Implemented");
    }

    public IActionResult LoadProductsBySeries(string arg) {
        List<Product> model = unit.GetProductsBySeries(arg).ToList();
        return PartialView("_ProductsPartial", model);
    }

    public IActionResult LoadCustomizations(string arg) {
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
        
        return PartialView("_CustomizationsPartial", model);
    }
}
