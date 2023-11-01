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
    public string ImgUrl { get; set; }
    public bool Hidden { get; set; }
    public bool IsOption { get; set; }
    public bool IsDrink { get; set; }

    /// <summary>
    /// Constructor for the Product class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="price"></param>
    /// <param name="series"></param>
    /// <param name="imgUrl"></param>
    /// <param name="hidden"></param>
    /// <param name="isOption"></param>
    /// <param name="isMain"></param>
    public Product(int id, string name, double price, string series, string imgUrl, bool hidden, bool isOption, bool isDrink) {
        Id = id;
        Name = name;
        Price = price;
        Series = series;
        ImgUrl = imgUrl;
        Hidden = hidden;
        IsOption = isOption;
        IsDrink = isDrink;
    }
}
