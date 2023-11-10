/*
    File: Cart.cs
    Model: Cart
    Author: Griffin Beaudreau
    Date: November 10, 2023
    Purpose: This file contains the Cart model, which is used to store the items
        that the user has added to their cart.
*/

namespace WebApp.Models.Cart;
public class Cart {
    public static List<Item> Items { get; set; } = new List<Item>();

    public static void AddItem(Item item) {
        foreach (Item i in Items) {
            if (i.Product.Id == item.Product.Id && i.Options == item.Options) {
                i.Quantity += item.Quantity;
                return;
            }
        }

        Items.Add(item);
    }
    public static void RemoveItem(Item item) { Items.Remove(item); }
    public static void Clear() { Items.Clear(); }

    public static double TotalCost() {
        double total = 0;
        foreach (Item item in Items) {
            total += item.Price();
        }

        return total;
    }
}
