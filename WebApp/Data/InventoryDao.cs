/*
    File: InventoryDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for managing Inventory entities.
/// </summary>
public class InventoryDao : IDao<Inventory> {
    
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler for executing database commands.</param>
    public InventoryDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public Inventory Get(int id) {
        var query = $"SELECT * FROM inventory WHERE id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null || !reader.HasRows) {
            reader?.Close();
            return new Inventory(-1, -1, -1, -1);
        }

        reader.Read();
        Inventory inventory = new(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetInt32(2),
            reader.GetInt32(3)
        );

        reader.Close();
        return inventory;
    }

    /// <inheritdoc/>
    public IEnumerable<Inventory> GetAll() {
        var query = "SELECT * FROM inventory ORDER BY id";
        var reader = commandHandler.ExecuteReader(query);

        List<Inventory> inventory = new();

        while (reader?.Read() == true) {
            inventory.Add(new Inventory(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3)
            ));
        }

        reader?.Close();
        return inventory;
    }

    /// <inheritdoc/>
    public void Add(Inventory t) {
        string sattement = (
            $"INSERT INTO inventory (ingredient_id, quantity, fill_level) " +
            $"VALUES ({t.IngredientId}, {t.Quantity}, {t.FillLevel})"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(Inventory t, Inventory newT) {
        string statement = (
            $"UPDATE inventory SET " +
            $"ingredient_id = {newT.IngredientId}, " +
            $"quantity = {newT.Quantity}, " +
            $"fill_level = {newT.FillLevel} " +
            $"WHERE id = {t.Id}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <inheritdoc/>
    public void Delete(Inventory t) {
        string statement = $"DELETE FROM inventory WHERE id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }

    public Inventory GetRecentInventory() {
        string query = $"SELECT * FROM inventory ORDER BY id DESC LIMIT 1";
        var reader = commandHandler.ExecuteReader(query);
        List<Inventory> inventory = new();

        while (reader?.Read() == true) {
            inventory.Add(new Inventory(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3)
            ));
        }

        reader?.Close();
        return inventory.ElementAt(0);
    }
}
