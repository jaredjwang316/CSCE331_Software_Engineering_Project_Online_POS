/*
    File: ProductIngredientsDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models;

namespace WebApp.Data;

public class ProductIngredientsDao : IDao<ProductIngredients> {
    private readonly CommandHandler commandHandler;

    public ProductIngredientsDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

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

    public void Add(ProductIngredients t) {
        string sattement = (
            $"INSERT INTO product_ingredients (product_id, ingredient_ids) " +
            $"VALUES ({t.ProductId}, ARRAY[{string.Join(",", t.IngredientIds)}])"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    public void Update(ProductIngredients t, ProductIngredients newT) {
        string statement = (
            $"DELETE FROM product_ingredients WHERE product_id = {t.ProductId}"
        );
        
        commandHandler.ExecuteNonQuery(statement);

        string sattement = (
            $"INSERT INTO product_ingredients (product_id, ingredient_ids) " +
            $"VALUES ({newT.ProductId}, ARRAY[{string.Join(",", newT.IngredientIds)}])"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    public void Delete(ProductIngredients t) {
        string statement = $"DELETE FROM product_ingredients WHERE product_id = {t.ProductId}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
