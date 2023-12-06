/*
    File: ProductIngredientsDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for handling operations related to <see cref="ProductIngredients"/>.
/// </summary>
/// <seealso cref="IDao{ProductIngredients}"/>
public class ProductIngredientsDao : IDao<ProductIngredients> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductIngredientsDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler used for database operations.</param>
    public ProductIngredientsDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public ProductIngredients Get(int id) {
        var query = $"SELECT * FROM product_ingredients WHERE product_id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new ProductIngredients(-1, new List<int>());
        }

        reader.Read();
        ProductIngredients productIngredients = new(
            reader.GetInt32(0),
            reader.GetFieldValue<int[]>(1).ToList()
        );

        reader?.Close();
        return productIngredients;
    }

    /// <inheritdoc/>
    public IEnumerable<ProductIngredients> GetAll() {
        var query = "SELECT * FROM product_ingredients";
        var reader = commandHandler.ExecuteReader(query);

        List<ProductIngredients> productIngredients = new();

        while (reader?.Read() == true) {
            productIngredients.Add(new ProductIngredients(
                reader.GetInt32(0),
                reader.GetFieldValue<int[]>(1).ToList()
            ));
        }

        reader?.Close();
        return productIngredients;
    }

    /// <inheritdoc/>
    public void Add(ProductIngredients t) {
        string sattement = (
            $"INSERT INTO product_ingredients (product_id, ingredient_ids) " +
            $"VALUES ({t.ProductId}, ARRAY[{string.Join(",", t.IngredientIds)}])"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(ProductIngredients t, ProductIngredients newT) {
        string sattement = (
            $"UPDATE product_ingredients SET " +
            $"ingredient_ids = ARRAY[{string.Join(",", newT.IngredientIds)}] " +
            $"WHERE product_id = {t.ProductId}"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Delete(ProductIngredients t) {
        string statement = $"DELETE FROM product_ingredients WHERE product_id = {t.ProductId}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
