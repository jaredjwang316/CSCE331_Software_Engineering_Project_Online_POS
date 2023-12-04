/*
    File: Cart.cs
    Model: Cart
    Author: Griffin Beaudreau
    Date: November 10, 2023
    Purpose: This file contains the Cart model, which is used to store the items
        that the user has added to their cart.
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Models.Cart;

/// <summary>
/// Represents a shopping cart that stores items added by the user.
/// </summary>
public class Cart {
    /// <summary>
    /// Gets or sets the list of items in the cart.
    /// </summary>
    public List<Item> Items { get; set; } = new List<Item>();
    
    /// <summary>
    /// Gets the total cost of all items in the cart.
    /// </summary>
    public double Total => TotalCost();

    // Constructor
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Cart"/> class.
    /// </summary>
    public Cart() { }

    /// <summary>
    /// Adds an item to the cart. If the same item with the same options exists,
    /// updates the quantity instead.
    /// </summary>
    /// <param name="item">The item to add to the cart.</param>
    public void AddItem(Item item) {
        // Check if item is already in cart with same options.
        foreach (Item i in Items) {
            if (i.Product.Id == item.Product.Id) {
                if (i.Options.Count == item.Options.Count) {
                    bool same = true;
                    for (int j = 0; j < i.Options.Count; j++) {
                        if (i.Options[j].Id != item.Options[j].Id) {
                            same = false;
                            break;
                        }
                    }
                    if (same) {
                        i.Quantity += item.Quantity;
                        return;
                    }
                }
            }
        }

        Items.Add(item);
    }
    
    /// <summary>
    /// Removes the specified item from the cart.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void RemoveItem(Item item) { Items.Remove(item); }
    
    /// <summary>
    /// Removes the item at the specified index from the cart.
    /// </summary>
    /// <param name="index">The index of the item to remove.</param>
    public void RemoveItem(int index) {
        if (index < Items.Count) {
            Items.RemoveAt(index);
        }
    }
    
    /// <summary>
    /// Clears all items from the cart.
    /// </summary>
    public void Clear() { Items.Clear(); }

    /// <summary>
    /// Calculates and returns the total cost of all items in the cart.
    /// </summary>
    /// <returns>The total cost of items in the cart.</returns>
    public double TotalCost() {
        double total = 0;
        foreach (Item item in Items) {
            total += item.Price();
        }

        return total;
    }
}
