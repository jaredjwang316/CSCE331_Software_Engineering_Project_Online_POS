/*
    File: CommandHandler.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using Npgsql;

namespace WebApp.Data;
/// <summary>
/// Handles the execution of database commands using Npgsql.
/// </summary>
public class CommandHandler {
    private readonly NpgsqlConnection? connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified NpgsqlConnection.
    /// </summary>
    /// <param name="connection">The NpgsqlConnection used for executing commands.</param>
    public CommandHandler(NpgsqlConnection connection) {
        this.connection = connection;
    }

    /// <summary>
    /// Executes a database query that returns a data reader.
    /// </summary>
    /// <param name="query">The SQL query string to be executed.</param>
    /// <returns>A NpgsqlDataReader containing the result of the query, or null in case of an error.</returns>
    public NpgsqlDataReader? ExecuteReader(string query) {
        NpgsqlDataReader? reader = null;

        try {
            var command = new NpgsqlCommand(query, connection);
            reader = command.ExecuteReader();
        } catch (Exception e) {
            Console.WriteLine($"Error executing query: {e.Message}");
        }

        return reader;
    }

    /// <summary>
    /// Executes a non-query SQL statement, such as an INSERT, UPDATE, or DELETE statement.
    /// </summary>
    /// <param name="statement">The SQL statement to be executed.</param>
    public void ExecuteNonQuery(string statement) {
        try {
            var command = new NpgsqlCommand(statement, connection);
            command.ExecuteNonQuery();
        } catch (Exception e) {
            Console.WriteLine($"Error executing statement: {e.Message}");
        }
    }
}