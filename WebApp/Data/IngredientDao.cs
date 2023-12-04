/*
    File: IngredientDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for managing Ingredient entities.
/// </summary>
public class IngredientDao : IDao<Ingredient> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="IngredientDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler for executing database commands.</param>
    public IngredientDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public Ingredient Get(int id) {
        var query = $"SELECT * FROM ingredients WHERE id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new Ingredient(-1, "null");
        }

        reader.Read();
        Ingredient ingredient = new(
            reader.GetInt32(0),
            reader.GetString(1)
        );

        reader.Close();
        return ingredient;
    }

    /// <inheritdoc/>
    public IEnumerable<Ingredient> GetAll() {
        var query = "SELECT * FROM ingredients";
        var reader = commandHandler.ExecuteReader(query);

        List<Ingredient> ingredients = new();

        while (reader?.Read() == true) {
            ingredients.Add(new Ingredient(
                reader.GetInt32(0),
                reader.GetString(1)
            ));
        }

        reader?.Close();
        return ingredients;
    }

    /// <inheritdoc/>
    public void Add(Ingredient t) {
        string sattement = (
            $"INSERT INTO ingredients (name) " +
            $"VALUES ('{t.Name}')"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(Ingredient t, Ingredient newT) {
        string statement = (
            $"UPDATE ingredients SET " +
            $"name = '{newT.Name}' " +
            $"WHERE id = {t.Id}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <inheritdoc/>
    public void Delete(Ingredient t) {
        string statement = $"DELETE FROM ingredients WHERE id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
