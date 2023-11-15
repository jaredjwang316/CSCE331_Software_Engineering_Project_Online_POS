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
public class Cart {
    public List<Item> Items { get; set; } = new List<Item>();

    // Constructor
    public Cart() { }

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
