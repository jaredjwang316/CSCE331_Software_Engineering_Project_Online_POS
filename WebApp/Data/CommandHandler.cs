/*
    File: CommandHandler.cs
    Author: Griffin Beaudreau
    Date: November 5, 2023
*/

using Npgsql;

namespace WebApp.Data;
public class CommandHandler {
    private readonly NpgsqlConnection? connection;

    public CommandHandler(NpgsqlConnection connection) {
        this.connection = connection;
    }

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

    public void ExecuteNonQuery(string statement) {
        Console.WriteLine(statement);
        try {
            var command = new NpgsqlCommand(statement, connection);
            command.ExecuteNonQuery();
        } catch (Exception e) {
            Console.WriteLine($"Error executing statement: {e.Message}");
        }
    }
}