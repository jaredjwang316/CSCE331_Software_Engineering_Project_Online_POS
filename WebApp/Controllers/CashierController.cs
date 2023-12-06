/*
    File: Controllers/CashierController.cs
    Author: Mrinal Yadav
    Date: 12/1
    Purpose: This file contains the CashierController class, which manages actions and
        functionalities specifically tailored for the Point of Sale (POS) system.

    Description:

    The CashierController class is an essential component of the web application's backend,
    dedicated to handling operations and requests related to the Point of Sale (POS) functionalities.
    It is designed to ensure secure access and execution of actions primarily intended for users
    with roles assigned as 'Cashier' or 'Manager'.

*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models.ViewModels;
using WebApp.Data;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;
using System.Security.Claims;

namespace WebApp.Controllers;

[Authorize(Roles = "Cashier, Manager"), ApiController]
public class CashierController : Controller
{
    private readonly ILogger<CashierController> _logger;
    private readonly IMemoryCache cache;
    private readonly CartService cartService;
    public CashierController(ILogger<CashierController> logger, IMemoryCache cache, CartService cartService)
    {
        _logger = logger;
        this.cache = cache;
        this.cartService = cartService;
    }

    [HttpGet, Route("Cashier/")]
    public IActionResult Index()
    {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;
        return View();
    }

    [HttpGet, Route("Cashier/Error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet, Route("Cashier/LoadCategories")]
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


    [HttpGet, Route("Cashier/LoadProductsBySeries")]
    public IActionResult LoadProductsBySeries(string arg) {
        UnitOfWork unit = new();
        List<Product> model = unit.GetProductsBySeries(arg).ToList();
        unit.CloseConnection();
        return PartialView("_ProductsPartial", model);
    }

    [HttpGet, Route("Cashier/LoadCustomizations")]
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
