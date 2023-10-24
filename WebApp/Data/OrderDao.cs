/*
    File: OrderDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models;

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
        string sattement = (
            $"INSERT INTO orders_final (employee_id, customer_name, order_date, total_order, item_ids) " +
            $"VALUES ({t.EmployeeId}, '{t.CustomerName}', '{t.OrderDate}', {t.TotalPrice}, ARRAY[{string.Join(",", t.ItemIds)}])"
        );
        commandHandler.ExecuteNonQuery(sattement);
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
}
