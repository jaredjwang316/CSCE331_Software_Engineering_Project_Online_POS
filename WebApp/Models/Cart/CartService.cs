/*
    File: Models/Cart/CartService.cs
    Author: Griffin Beaudreau
    Date: November 25, 2023
    Purpose: This file contains the CartService class, which is used to store the cart
        in the user's session.
*/

using Newtonsoft.Json;

namespace WebApp.Models.Cart;

/// <summary>
/// Provides functionality to interact with the user's shopping cart stored in the session.
/// </summary>
public class CartService {
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    public CartService(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Retrieves the user's shopping cart from the session.
    /// If the cart is not found, a new cart is created and stored in the session.
    /// </summary>
    /// <returns>The user's shopping cart.</returns>
    public Cart GetCartFromSession() {
        var session = _httpContextAccessor.HttpContext!.Session;
        if (session.GetString("Cart") == null) {
            Cart cart = new();
            SetCartInSession(cart);
        }
        
        var serializedCart = session.GetString("Cart")!;
        return JsonConvert.DeserializeObject<Cart>(serializedCart)!;
    }

    /// <summary>
    /// Sets the specified shopping cart in the session.
    /// </summary>
    /// <param name="cart">The shopping cart to store in the session.</param>
    public void SetCartInSession(Cart cart) {
        var session = _httpContextAccessor.HttpContext!.Session;
        var serializedCart = JsonConvert.SerializeObject(cart);
        session.SetString("Cart", serializedCart);
    }
}
