/*
    File: Inventory.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the Inventory table.
/// </summary>
public class Inventory {
    /// <summary>
    /// Gets or sets the unique identifier for the inventory item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the associated ingredient.
    /// </summary>
    public int IngredientId { get; set; }
    
    /// <summary>
    /// Gets or sets the quantity of the ingredient in the inventory.
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Gets or sets the fill level of the inventory.
    /// </summary>
    public int FillLevel { get; set; }

    /// <summary>
    /// Constructor for the Inventory class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ingredientId"></param>
    /// <param name="quantity"></param>
    /// <param name="fillLevel"></param>
    public Inventory(int id, int ingredientId, int quantity, int fillLevel) {
        Id = id;
        IngredientId = ingredientId;
        Quantity = quantity;
        FillLevel = fillLevel;
    }
}
