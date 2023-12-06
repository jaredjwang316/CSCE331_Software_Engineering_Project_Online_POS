/*
    File: Item.cs
    Model: Cart
    Author: Griffin Beaudreau
    Date: November 10, 2023
    Purpose: This file contains the Item model, which is used to represent an item
        in the user's cart.
*/

using WebApp.Data;
using WebApp.Models.UnitOfWork;

namespace WebApp.Models.Cart;

/// <summary>
/// Represents an item in the user's shopping cart.
/// </summary>
public class Item {
    /// <summary>
    /// Gets or sets the product associated with the item.
    /// </summary>
    public Product Product { get; set; }
    
    /// <summary>
    /// Gets or sets the list of product options associated with the item.
    /// </summary>
    public List<Product> Options { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of the item.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class.
    /// </summary>
    /// <param name="product">The main product associated with the item.</param>
    /// <param name="options">The list of product options associated with the item.</param>
    /// <param name="quantity">The quantity of the item.</param>
    public Item(Product product, List<Product> options, int quantity) {
        Product = product;
        Quantity = quantity;
        Options = options;
    }

    /// <summary>
    /// Calculates and returns the total price of the item.
    /// </summary>
    /// <returns>The total price of the item.</returns>
    public double Price() {
        double price = Product.Price;
        foreach (Product option in Options) {
            price += option.Price;
        }

        return price * Quantity;
    }
}
