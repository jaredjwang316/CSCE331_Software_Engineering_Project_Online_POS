/*
    File: ProductIngredients.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the ProductIngredients table.
/// </summary>
public class ProductIngredients {
    public int ProductId { get; set; }
    public List<int> IngredientIds { get; set; }

    /// <summary>
    /// Constructor for the ProductIngredients class.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="ingredientIds"></param>
    public ProductIngredients(int productId, List<int> ingredientIds) {
        ProductId = productId;
        IngredientIds = ingredientIds;
    }
}
