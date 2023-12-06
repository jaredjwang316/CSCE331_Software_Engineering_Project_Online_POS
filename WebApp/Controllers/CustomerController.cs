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
/// <summary>
/// Controller responsible for managing customer-related operations, including viewing products, categories, best sellers, and favorites.
/// </summary>
[ApiController]
public class CustomerController : Controller {
    private readonly CartService cartService;
    private readonly IMemoryCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerController"/> class.
    /// </summary>
    /// <param name="cartService">The cart service instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    public CustomerController(CartService cartService, IMemoryCache cache) {
        this.cartService = cartService;
        this.cache = cache;
    }

    /// <summary>
    /// Displays the default view for the customer controller, showing the cart items count.
    /// </summary>
    /// <returns>The default view.</returns>
    [HttpGet, Route("Customer/")]
    public IActionResult Index() {
        Cart cart = cartService.GetCartFromSession();
        int itemsInCart = cart!.Items.Sum(i => i.Quantity);
        ViewBag.itemsInCart = itemsInCart;

        return View();
    }

    /// <summary>
    /// Loads and returns the product categories from the cache or database.
    /// </summary>
    /// <returns>The partial view containing the loaded product categories.</returns>
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

    /// <summary>
    /// Loads and returns the best-selling products from the cache or database.
    /// </summary>
    /// <returns>The partial view containing the loaded best-selling products.</returns>
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

    /// <summary>
    /// Loads and returns the user's favorite products.
    /// </summary>
    /// <returns>The partial view containing the loaded favorite products.</returns>
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

    /// <summary>
    /// Loads and returns products based on the specified series name.
    /// </summary>
    /// <param name="arg">The series name.</param>
    /// <returns>The partial view containing the loaded products.</returns>
    [HttpGet, Route("Customer/LoadProductsBySeries")]
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

    /// <summary>
    /// Adds a product to the user's favorites.
    /// </summary>
    /// <param name="productID">The ID of the product to be added to favorites.</param>
    /// <returns>The result of the operation.</returns>
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
    /// <summary>
    /// Controller action for removing a product from the user's favorites.
    /// </summary>
    /// <param name="productID">The ID of the product to be removed from favorites.</param>
    /// <returns>The result of the operation and an error if it can't be executed.</returns>

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
