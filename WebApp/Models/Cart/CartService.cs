/*
    File: Models/Cart/CartService.cs
    Author: Griffin Beaudreau
    Date: November 25, 2023
    Purpose: This file contains the CartService class, which is used to store the cart
        in the user's session.
*/

using Newtonsoft.Json;

namespace WebApp.Models.Cart;

public class CartService {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    public Cart GetCartFromSession() {
        var session = _httpContextAccessor.HttpContext!.Session;
        if (session.GetString("Cart") == null) {
            Cart cart = new();
            SetCartInSession(cart);
        }
        
        var serializedCart = session.GetString("Cart")!;
        return JsonConvert.DeserializeObject<Cart>(serializedCart)!;
    }

    public void SetCartInSession(Cart cart) {
        var session = _httpContextAccessor.HttpContext!.Session;
        var serializedCart = JsonConvert.SerializeObject(cart);
        session.SetString("Cart", serializedCart);
    }
}
