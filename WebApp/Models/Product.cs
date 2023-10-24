/*
    File: Product.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models;

/// <summary>
/// This class is used to represent a row in the Product table.
/// </summary>
public class Product {
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Series { get; set; }

    /// <summary>
    /// Constructor for the Product class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="price"></param>
    /// <param name="series"></param>
    public Product(int id, string name, double price, string series) {
        Id = id;
        Name = name;
        Price = price;
        Series = series;
    }
}
