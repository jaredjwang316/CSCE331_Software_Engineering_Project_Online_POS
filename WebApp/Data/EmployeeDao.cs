/*
    File: EmployeeDao.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;
/// <summary>
/// Data Access Object (DAO) for managing Employee entities in the database.
/// </summary>
public class EmployeeDao : IDao<Employee> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeDao"/> class with the specified <see cref="CommandHandler"/>.
    /// </summary>
    /// <param name="commandHandler">The CommandHandler used to execute SQL commands.</param>
    public EmployeeDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <inheritdoc/>
    public Employee Get(int id) {
        var query = $"SELECT * FROM employees WHERE id = {id}";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null) {
            return new Employee(-1, "null", "null", false, "null");
        }

        reader.Read();
        Employee employee = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2), 
            reader.GetBoolean(3),
            reader.GetString(4)
        );

        reader.Close();
        return employee;
    }

    /// <inheritdoc/>
    public IEnumerable<Employee> GetAll() {
        var query = "SELECT * FROM employees";
        var reader = commandHandler.ExecuteReader(query);

        List<Employee> employees = new();

        while (reader?.Read() == true) {
            employees.Add(new Employee(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2), 
                reader.GetBoolean(3),
                reader.GetString(4)
            ));
        }

        reader?.Close();
        return employees;
    }

    /// <inheritdoc/>
    public void Add(Employee t) {
        string sattement = (
            $"INSERT INTO employees (name, password, isManager, email) " +
            $"VALUES ('{t.Name}', '{t.Password}', {t.IsManager}, '{t.Email}')"
        );
        commandHandler.ExecuteNonQuery(sattement);
    }

    /// <inheritdoc/>
    public void Update(Employee t, Employee newT) {
        string statement = (
            $"UPDATE employees SET " +
            $"name = '{newT.Name}', " +
            $"password = '{newT.Password}', " +
            $"isManager = {newT.IsManager}, " +
            $"email = '{newT.Email}' " +
            $"WHERE id = {t.Id}"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <inheritdoc/>
    public void Delete(Employee t) {
        string statement = $"DELETE FROM employees WHERE id = {t.Id}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
