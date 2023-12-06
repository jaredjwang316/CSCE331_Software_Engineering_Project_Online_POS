/*
    File: OrderDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

public class OrderDao : IDao<Order> {
    private readonly CommandHandler commandHandler;

    public OrderDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    public Order Get(int id) {
        var query = $"SELECT * FROM orders_final WHERE order_id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new Order(-1, -1, "null", new DateTime(), -1.0, new List<int>());
        }

        reader.Read();
        Order order = new(
            reader.GetInt32(0),
            reader.GetInt32(1),
            reader.GetString(2),
            reader.GetDateTime(3),
            reader.GetDouble(4),
            reader.GetFieldValue<int[]>(5).ToList()
        );

        reader?.Close();
        return order;
    }

    public IEnumerable<Order> GetAll() {
        var query = "SELECT * FROM orders_final";
        var reader = commandHandler.ExecuteReader(query);

        List<Order> orders = new();

        while (reader?.Read() == true) {
            orders.Add(new Order(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetDouble(4),
                reader.GetFieldValue<int[]>(5).ToList()
            ));
        }

        reader?.Close();
        return orders;
    }

    public void Add(Order t) {
        // foreach (int id in t.ItemIds) {
        //      string q = (
        //          $"WITH ProductIngredientsCTE AS ( " +
        //          $"SELECT unnest(ingredient_ids) AS ingredient_id " +
        //          $"FROM product_ingredients " +
        //          $"WHERE product_id = {id} ) " +
        
        //          $"UPDATE inventory " +
        //          $"SET quantity = quantity - 1 " +
        //          $"WHERE ingredient_id IN (SELECT ingredient_id FROM ProductIngredientsCTE); "
        //      );
        //      commandHandler.ExecuteNonQuery(q);
        // }

        string ids = string.Join(",", t.ItemIds);
        string updateIngredientsStatement = (
            $"WITH ProductIngredientsCTE AS ( " +
            $"SELECT unnest(ingredient_ids) AS ingredient_id " +
            $"FROM product_ingredients " +
            $"WHERE product_id IN ({ids}) ) " +
        
            $"UPDATE inventory " +
            $"SET quantity = quantity - 1 " +
            $"WHERE ingredient_id IN (SELECT ingredient_id FROM ProductIngredientsCTE); "
        );
        commandHandler.ExecuteNonQuery(updateIngredientsStatement);

        // Also update options from inventory

        // Get all products from products table where is_option is true and product_id is in t.ItemIds
        string getOptionsQuery = (
            $"SELECT * " +
            $"FROM products " +
            $"WHERE is_option = true AND id IN ({ids})"
        );
        var reader = commandHandler.ExecuteReader(getOptionsQuery);
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

        // From inventory change quantity of options. Account for quantity in cart
        // key value pair for id and quantity
        Dictionary<string, int> productCounts = new();
        foreach (int id in t.ItemIds) {
            if (products.Any(p => p.Id == id)) {
                string name = products.FirstOrDefault(p => p.Id == id)!.Name.ToLower();
                if (productCounts.ContainsKey(name)) {
                    productCounts[name]++;
                } else {
                    productCounts.Add(name, 1);
                }
            }
        }

        // reduce quantity by value in productCounts
        var updateStatements = productCounts.Select(pc =>
            $"UPDATE inventory " +
            $"SET quantity = quantity - {pc.Value} " +
            $"WHERE ingredient_name = '{pc.Key}'"
        );

        string updateOptionsStatement = string.Join("; ", updateStatements);
        commandHandler.ExecuteNonQuery(updateOptionsStatement);
        
        string statement = (
            $"INSERT INTO orders_final (employee_id, customer_name, order_date, total_order, item_ids) " +
            $"VALUES ({t.EmployeeId}, '{t.CustomerName}', '{t.OrderDate}', {t.TotalPrice}, ARRAY[{string.Join(",", t.ItemIds)}])"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Update(Order t, Order newT) {
        string statement = (
            $"UPDATE orders_final SET " +
            $"employee_id = {newT.EmployeeId}, " +
            $"customer_name = '{newT.CustomerName}', " +
            $"order_date = '{newT.OrderDate}', " +
            $"total_order = {newT.TotalPrice}, " +
            $"item_ids = ARRAY[{string.Join(",", newT.ItemIds)}] " +
            $"WHERE order_id = {t.Id}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Delete(Order t) {
        string statement = $"DELETE FROM orders_final WHERE order_id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }

    public List<Order> GetOrdersBetween(DateTime starttime, DateTime endtime) {
        var query = $"SELECT * FROM orders_final WHERE order_date between '{starttime}' AND '{endtime}'";

        var reader = commandHandler.ExecuteReader(query);

        List<Order> orders = new();
        if (reader == null) {
            return orders;
        }


        while (reader?.Read() == true) {
            orders.Add(new Order(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetDouble(4),
                reader.GetFieldValue<int[]>(5).ToList()
            ));
        }

        reader?.Close();
        return orders;
        
    }
    public List<(string, string, int)> GetSalesTogether(DateTime start_date, DateTime end_date) {
        List<(string,string, int)> orders = new();
        var query = $"WITH ItemPairs AS (\n" +
            $"  SELECT\n" +
            $"    a.item_id AS item1," +
            $"    b.item_id AS item2\n" +
            $"  FROM\n" +
            $"    orders_final,\n" +
            $"    UNNEST(item_ids) AS a(item_id),\n" +
            $"    UNNEST(item_ids) AS b(item_id)\n" +
            $"  WHERE\n" +
            $"    a.item_id < b.item_id\n" +
            $"    AND a.item_id >= 1 AND a.item_id <= 71\n" +
            $"    AND b.item_id >= 1 AND b.item_id <= 71\n" +
            $"    AND order_date >= '{ start_date }' AND order_date < '{ end_date }'\n" +
            $")\n" +
            $"SELECT\n" +
            $"  p1.name AS item1_name,\n" + 
            $"  p2.name AS item2_name,\n" + 
            $"  COUNT(*) AS frequency\n" +
            $"FROM\n" + 
            $"  ItemPairs\n" + 
            $"JOIN\n" + 
            $"  products AS p1 ON ItemPairs.item1 = p1.id\n" + 
            $"JOIN\n" + 
            $"  products AS p2 ON ItemPairs.item2 = p2.id\n" +
            $"GROUP BY\n" +
            $"  p1.name,\n" + 
            $"  p2.name\n" +
            $"ORDER BY\n" + 
            $"  frequency DESC;";
        
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return orders;
        }


        while (reader?.Read() == true) {
            orders.Add((reader.GetString(0), reader.GetString(1), reader.GetInt32(2)));
        }

        reader?.Close();
        return orders;
    }
}
