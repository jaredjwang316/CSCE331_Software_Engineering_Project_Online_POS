/*
    File: Item.cs
    Model: Cart
    Author: Griffin Beaudreau
    Date: November 10, 2023
    Purpose: This file contains the Item model, which is used to represent an item
        in the user's cart.
*/

using WebApp.Data;

namespace WebApp.Models.Cart;
public class Item {
    public Product Product { get; set; }
    public List<Product> Options { get; set; }
    public int Quantity { get; set; }

    public Item(Product product, List<Product> options, int quantity) {
        Product = product;
        Quantity = quantity;
        Options = options;
    }

    public double Price() {
        double price = Product.Price;
        foreach (Product option in Options) {
            price += option.Price;
        }

        return price * Quantity;
    }
}
