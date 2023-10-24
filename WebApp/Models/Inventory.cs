/*
    File: Inventory.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models;

/// <summary>
/// This class is used to represent a row in the Inventory table.
/// </summary>
public class Inventory {
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public int Quantity { get; set; }
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
