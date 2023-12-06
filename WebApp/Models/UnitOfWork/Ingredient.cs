/*
    File: Ingredient.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the Ingredient table.
/// </summary>
public class Ingredient {
    /// <summary>
    /// Gets or sets the unique identifier for the inventory item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the associated ingredient.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Constructor for the Ingredient class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Ingredient(int id, string name) {
        Id = id;
        Name = name;
    }
}
