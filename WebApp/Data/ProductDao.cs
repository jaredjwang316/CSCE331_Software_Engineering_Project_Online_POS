/*
    File: ProductDao.cs
    Author: Griffin Beaudreau
    Date: 10/23/2023
    Description: DAO class for the Product model.
*/

using WebApp.Models;

namespace WebApp.Data;

public class ProductDao : IDao<Product> {
    private readonly CommandHandler commandHandler;

    public ProductDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    public Product Get(int id) {
        var query = $"SELECT * FROM products WHERE id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new Product(-1, "null", -1, "null");
        }

        reader.Read();
        Product product = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetDouble(2), 
            reader.GetString(3)
        );

        reader.Close();
        return product;
    }

    public IEnumerable<Product> GetAll() {
        var query = "SELECT * FROM products";
        var reader = commandHandler.ExecuteReader(query);

        List<Product> products = new();

        while (reader?.Read() == true) {
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDouble(2), 
                reader.GetString(3)
            ));
        }

        reader?.Close();
        return products;
    }

    public void Add(Product t) {
        string sattement = (
            $"INSERT INTO products (name, price, series) " +
            $"VALUES ('{t.Name}', {t.Price}, '{t.Series}')"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    public void Update(Product t, Product newT) {
        string statement = (
            $"UPDATE products SET " +
            $"name = '{newT.Name}', " +
            $"price = {newT.Price}, " +
            $"series = '{newT.Series}' " +
            $"WHERE id = {t.Id}"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Delete(Product t) {
        string statement = $"DELETE FROM products WHERE id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }

    //====================================================================================================
    // Custom Queries for the database
    //====================================================================================================

    public IEnumerable<Product> GetProductsBySeries(string series) {
        string query = $"SELECT * FROM products WHERE series = {series}";
        var reader = commandHandler.ExecuteReader(query);

        List<Product> products = new();

        while (reader?.Read() == true) {
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDouble(2), 
                reader.GetString(3)
            ));
        }

        reader?.Close();
        return products;
    }

    public IEnumerable<string> GetUniqueSeries() {
        string query = $"SELECT DISTINCT series FROM products";
        var reader = commandHandler.ExecuteReader(query);

        List<string> series = new();

        while (reader?.Read() == true) {
            series.Add(reader.GetString(0));
        }

        reader?.Close();
        return series;
    }
}
