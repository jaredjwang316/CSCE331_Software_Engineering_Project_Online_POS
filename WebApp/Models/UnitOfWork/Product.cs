/*
    File: Product.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the Product table.
/// </summary>
public class Product {
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public double Price { get; set; }
    
    /// <summary>
    /// Gets or sets the series to which the product belongs.
    /// </summary>
    public string Series { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the product's image.
    /// </summary>
    public string ImgUrl { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the product is hidden.
    /// </summary>
    public bool Hidden { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the product is an option.
    /// </summary>
    public bool IsOption { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the product is a drink.
    /// </summary>
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
