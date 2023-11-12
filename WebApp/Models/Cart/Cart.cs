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
    public List<Item> Items { get; set; } = new List<Item>();

    // Constructor
    public Cart() { }

    public void AddItem(Item item) {
        foreach (Item i in Items) {
            if (i.Product.Id == item.Product.Id) {
                bool match = true;
                if (item.Options.Count == 0) {
                    i.Quantity += item.Quantity;
                    return;
                }
                foreach (Product option in i.Options) {
                    if (!item.Options.Any(o => o.Name == option.Name)) {
                        match = false;
                        break;
                    }
                }
                if (match) {
                    i.Quantity += item.Quantity;
                    return;
                }
            }
        }
        Items.Add(item);
    }
    public void RemoveItem(Item item) { Items.Remove(item); }
    public void RemoveItem(int index) {
        if (index < Items.Count) {
            Items.RemoveAt(index);
        }
    }
    public void Clear() { Items.Clear(); }

    public double TotalCost() {
        double total = 0;
        foreach (Item item in Items) {
            total += item.Price();
        }

        return total;
    }
}
