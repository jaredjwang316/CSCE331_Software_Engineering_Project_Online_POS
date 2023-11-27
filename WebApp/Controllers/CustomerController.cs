/*
    File: Controllers/CustomerController.cs
    Author: Griffin Beaudreau
    Date: November 12, 2023
    Purpose: This file contains the CustomerController class, which is used to handle
        requests from the customer view.
*/

using WebApp.Models.UnitOfWork;
using WebApp.Models.ViewModels;
using WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;

namespace WebApp.Controllers;
public class CustomerController : Controller {
    private readonly UnitOfWork unit;
    private readonly CartService cartService;
    private readonly IMemoryCache cache;

    public CustomerController(UnitOfWork unit, CartService cartService, IMemoryCache cache) {
        this.unit = unit;
        this.cartService = cartService;
        this.cache = cache;
    }

    public IActionResult Index() {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        // Get and test latitude and longitude cookies
        string? latitude = Request.Cookies["latitude"];
        string? longitude = Request.Cookies["longitude"];
        Console.WriteLine($"Latitude: {latitude}, Longitude: {longitude}");

        return View();
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
