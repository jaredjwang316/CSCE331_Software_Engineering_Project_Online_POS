/*
    File: Ingredient.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models;

/// <summary>
/// This class is used to represent a row in the Ingredient table.
/// </summary>
public class Ingredient {
    public int Id { get; set; }
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
