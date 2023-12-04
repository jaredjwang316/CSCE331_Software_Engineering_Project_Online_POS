/*
    File: ProductDao.cs
    Author: Griffin Beaudreau
    Date: 10/23/2023
    Description: DAO class for the Product model.
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for managing Product entities.
/// </summary>
public class ProductDao : IDao<Product> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler for executing database commands.</param>
    public ProductDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public Product Get(int id) {
        var query = $"SELECT * FROM products WHERE id IN ({id})";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null || !reader.HasRows) {
            reader?.Close();
            return new Product(-1, "null", -1, "null", "null", false, false, false);
        }

        reader.Read();
        Product product = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDouble(2), 
            reader.GetString(3),
            reader.GetString(4),
            reader.GetBoolean(5),
            reader.GetBoolean(6),
            reader.GetBoolean(7)
        );

        reader.Close();
        return product;
    }

    /// <inheritdoc/>
    public IEnumerable<Product> GetAll() {
        var query = "SELECT * FROM products ORDER BY series, name";
        var reader = commandHandler.ExecuteReader(query);

        List<Product> products = new();

        while (reader?.Read() == true) {
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDouble(2), 
                reader.GetString(3),
                reader.GetString(4),
                reader.GetBoolean(5),
                reader.GetBoolean(6),
                reader.GetBoolean(7)
            ));
        }

        reader?.Close();
        return products;
    }

    /// <inheritdoc/>
    public void Add(Product t) {
        string sattement = (
            $"INSERT INTO products (name, price, series, img_url, hidden, is_option, is_drink) " +
            $"VALUES (" +
                $"'{t.Name}', " +
                $"{t.Price}, " +
                $"'{t.Series}', " +
                $"'{t.ImgUrl}', " +
                $"{t.Hidden}, " +
                $"{t.IsOption})" +
                $"{t.IsDrink}"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(Product t, Product newT) {
        string statement = (
            $"UPDATE products SET " +
            $"name = '{newT.Name}', " +
            $"price = {newT.Price}, " +
            $"series = '{newT.Series}', " +
            $"img_url = '{newT.ImgUrl}', " +
            $"hidden = {newT.Hidden}, " +
            $"is_option = {newT.IsOption}, " +
            $"is_drink = {newT.IsDrink} " +
            $"WHERE id = {t.Id}"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <inheritdoc/>
    public void Delete(Product t) {
        string statement = $"DELETE FROM products WHERE id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }

    //====================================================================================================
    // Custom Queries for the database
    //====================================================================================================

    /// <summary>
    /// Gets products by series.
    /// </summary>
    /// <param name="series">The series of products to retrieve.</param>
    /// <returns>The products belonging to the specified series.</returns>
    public IEnumerable<Product> GetProductsBySeries(string series) {
        string query = $"SELECT * FROM products WHERE series = '{series}' ORDER BY name";
        var reader = commandHandler.ExecuteReader(query);

        List<Product> products = new();

        while (reader?.Read() == true) {
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDouble(2), 
                reader.GetString(3),
                reader.GetString(4),
                reader.GetBoolean(5),
                reader.GetBoolean(6),
                reader.GetBoolean(7)
            ));
        }

        reader?.Close();
        return products;
    }

    /// <summary>
    /// Gets unique product series based on specified criteria.
    /// </summary>
    /// <param name="includeDrinks">Include drinks in the result.</param>
    /// <param name="includeHidden">Include hidden products in the result.</param>
    /// <param name="includeIsOption">Include products marked as options in the result.</param>
    /// <returns>The unique product series based on the specified criteria.</returns>
    public IEnumerable<string> GetUniqueSeries(bool includeDrinks = true, bool includeHidden = false, bool includeIsOption = false) {
        
        string query = $"SELECT DISTINCT series FROM products WHERE " +
            $"is_drink = {includeDrinks} AND " +
            $"hidden = {includeHidden} AND " +
            $"is_option = {includeIsOption} " +
            $"ORDER BY series";

        var reader = commandHandler.ExecuteReader(query);

        List<string> series = new();

        while (reader?.Read() == true) {
            series.Add(reader.GetString(0));
        }

        reader?.Close();
        return series;
    }

    /// <summary>
    /// Gets a list of best-selling products based on the specified limit.
    /// </summary>
    /// <param name="limit">The maximum number of best-selling products to retrieve (default is 5).</param>
    /// <returns>A collection of best-selling products.</returns>
    public IEnumerable<Product> GetBestSellingProducts(int limit = 5) {
        string query = 
            $"SELECT item_id, COUNT(item_id) AS item_count " +
            $"FROM ( " +
                $"SELECT unnest(item_ids) AS item_id " +
                $"FROM orders_final " +
            $") AS item_counts " +
            $"WHERE item_id NOT IN (" +
                $"SELECT id FROM products WHERE is_drink = false" +
            $") " +
            $"GROUP BY item_id " +
            $"ORDER BY item_count DESC " +
            $"LIMIT {limit}";

        var reader = commandHandler.ExecuteReader(query);

        List<int> product_id = new();

        while (reader?.Read() == true) {
            product_id.Add(reader.GetInt32(0));
        }

        reader?.Close();

        List<Product> products = new();
        // List<Product> allProducts = (List<Product>) GetAll();
        
        foreach (int id in product_id) {
            // products.Add(allProducts.Find(product => product.Id == id)!);
            Product product = Get(id);
            if (product.Id != -1) {
                products.Add(product); // TODO: Fix this after adding image urls to the database
            }
        }

        return products;
    }
}
