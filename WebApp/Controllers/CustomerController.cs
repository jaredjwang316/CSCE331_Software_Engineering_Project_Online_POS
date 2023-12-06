/*
    File: Controllers/CustomerController.cs
    Author: Griffin Beaudreau
    Date: November 12, 2023
    Purpose: This file contains the CustomerController class, which is used to handle
        requests from the customer view.

    Description:  The CustomerController class is an integral part of the web application's backend,
    specifically designed to manage and respond to various requests originating from
    the customer-facing view. It interacts with the UnitOfWork, CartService, IMemoryCache,
    and other components to facilitate operations related to customer activities.
*/

using WebApp.Models.UnitOfWork;
using WebApp.Models.ViewModels;
using WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Cart;
using System.Security.Claims;

namespace WebApp.Controllers;
[ApiController]
public class CustomerController : Controller {
    private readonly CartService cartService;
    private readonly IMemoryCache cache;

    public CustomerController(CartService cartService, IMemoryCache cache) {
        this.cartService = cartService;
        this.cache = cache;
    }

    [HttpGet, Route("Customer/")]
    public IActionResult Index() {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        return View();
    }

    [HttpGet, Route("Customer/LoadCategories")]
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

    [HttpGet, Route("Customer/LoadBestSellers")]
    public IActionResult LoadBestSellers() {
        UnitOfWork unit = new();
        List<Product> model = cache.GetOrCreate("BestSellers", entry => {
            entry.SlidingExpiration = TimeSpan.FromMinutes(10);
            return unit.GetBestSellingProducts(10).ToList();
        })!;

        unit.CloseConnection();

        return PartialView("_ProductsPartial", model);
    }

    [HttpGet, Route("Customer/LoadFavorites")]
    public IActionResult LoadFavorites() {
        // Use _ProductsPartial when favorites are implemented

        if (!User.Identity!.IsAuthenticated) {
            return Content("You must be logged in to view favorites.");
        }

        UnitOfWork unit = new();
        string email = User.FindFirstValue(ClaimTypes.Email)!;
        User? user = unit.GetUser(email);
        if (user == null) {
            unit.CloseConnection();
            return Content("User not found.");
        }

        List<Product> model = unit.GetAll<Product>()
            .Where(product => user.Favorites.Any(favorite => favorite == product.Id))
            .ToList();
        
        unit.CloseConnection();

        return PartialView("_ProductsPartial", model);
    }

    [HttpGet, Route("Customer/LoadProductsBySeries")]
    public IActionResult LoadProductsBySeries(string arg) {
        UnitOfWork unit = new();
        List<Product> model = unit.GetProductsBySeries(arg).ToList();
        unit.CloseConnection();
        return PartialView("_ProductsPartial", model);
    }

    [HttpGet, Route("Customer/LoadCustomizations")]
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

    [HttpPost, Route("Customer/AddFavorite/{productID}")]
    public IActionResult AddFavorite(int productID) {
        if (!User.Identity!.IsAuthenticated) {
            return BadRequest("You must be logged in to add favorites.");
        }

        string email = User.FindFirstValue(ClaimTypes.Email)!;
        if (email == null) {
            return BadRequest("No email found for user.");
        }

        UnitOfWork unit = new();
        User? user = unit.GetUser(email);
        if (user == null) {
            unit.CloseConnection();
            return BadRequest("User not found.");
        }

        if (user.Favorites.Any(favorite => favorite == productID)) {
            unit.CloseConnection();
            return Ok();
        }

        int[] favorites = user.Favorites.Append(productID).ToArray();
        User newUser = new(user.Name, user.Email, favorites);
        unit.Update(user, newUser);
        unit.CloseConnection();

        return Ok();
    }

    [HttpDelete, Route("Customer/RemoveFavorite/{productID}")]
    public IActionResult RemoveFavorite(int productID) {
        if (!User.Identity!.IsAuthenticated) {
            return BadRequest("You must be logged in to remove favorites.");
        }

        string email = User.FindFirstValue(ClaimTypes.Email)!;
        if (email == null) {
            return BadRequest("No email found for user.");
        }

        UnitOfWork unit = new();
        User? user = unit.GetUser(email);
        if (user == null) {
            unit.CloseConnection();
            return BadRequest("User not found.");
        }

        if (!user.Favorites.Any(favorite => favorite == productID)) {
            unit.CloseConnection();
            return Ok();
        }

        int[] favorites = user.Favorites.Where(favorite => favorite != productID).ToArray();
        User newUser = new(user.Name, user.Email, favorites);
        unit.Update(user, newUser);
        unit.CloseConnection();

        return Ok();
    }
}
