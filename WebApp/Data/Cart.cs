/*
    File: Cart.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using WebApp.Models;

namespace WebApp.Data;

public class Cart {
    public List<CartItem> Item { get; set; }

    public Cart() {
        Item = new List<CartItem>();
    }

    public void AddItem(CartItem item) {
        Item.Add(item);
    }

    public void RemoveItem(CartItem item) {
        Item.Remove(item);
    }

    public void Clear() {
        Item.Clear();
    }

    public List<CartItem> GetItems() {
        return Item;
    }

    public void print() {
        foreach (CartItem item in Item) {
            Console.WriteLine(item.GetProduct().Name);
            foreach (Product customization in item.GetCustomizations()) {
                Console.WriteLine("  " + customization.Name);
            }
        }
    }
}

public class CartItem {
    public Product Product { get; set; }
    public List<Product> Customizations { get; set; }
    
    public CartItem(Product product, List<Product> customizations) {
        Product = product;
        Customizations = customizations;
    }

    public Product GetProduct() {
        return Product;
    }

    public List<Product> GetCustomizations() {
        return Customizations;
    }

    public double GetTotalPrice() {
        double total = Product.Price;
        foreach (Product customization in Customizations) {
            total += customization.Price;
        }
        return total;
    }
}