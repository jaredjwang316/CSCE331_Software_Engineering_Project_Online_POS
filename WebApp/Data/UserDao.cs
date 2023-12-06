/*
    File: UserDao.cs
    Author: Griffin Beaudreau
    Date: November 29, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

/// <summary>
/// Data Access Object (DAO) for managing user entities in the database.
/// </summary>
public class UserDao : IDao<User> {
    private readonly CommandHandler commandHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserDao"/> class.
    /// </summary>
    /// <param name="commandHandler">The command handler for executing database queries.</param>
    public UserDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    /// <summary>
    /// Not supported for user entities. Throws a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>Throws a <see cref="NotSupportedException"/>.</returns>
    public User Get(int id) {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Gets a user entity by email.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user entity with the specified email, or null if not found.</returns>
    public User? Get(string email) {
        var query = $"SELECT * FROM users WHERE email = '{email}'";
        var reader = commandHandler.ExecuteReader(query);

        if (reader == null || !reader.HasRows) {
            reader?.Close();
            return null;
        }

        reader.Read();
        User user = new(
            reader.GetString(0),
            reader.GetString(1),
            (int[])reader.GetValue(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetString(6)
        );

        reader?.Close();
        return user;
    }

    /// <summary>
    /// Gets all user entities from the database.
    /// </summary>
    /// <returns>An enumerable collection of all user entities.</returns>
    public IEnumerable<User> GetAll() {
        var query = "SELECT * FROM users";
        var reader = commandHandler.ExecuteReader(query);

        List<User> users = new();

        while (reader?.Read() == true) {
            users.Add(new User(
                reader.GetString(0),
                reader.GetString(1),
                (int[])reader.GetValue(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetString(6)
            ));
        }

        reader?.Close();
        return users;
    }

    /// <summary>
    /// Adds a new user entity to the database.
    /// </summary>
    /// <param name="t">The user entity to be added.</param>
    public void Add(User t) {
        string favorites = t.Favorites != null ? string.Join(",", t.Favorites) : string.Join(",", Array.Empty<int>());
        string statement = (
            $"INSERT INTO users (name, email, favorites, acc_cursor, acc_text_size, acc_contrast, acc_language) " +
            $"VALUES ('{t.Name}', '{t.Email}', ARRAY[{favorites}]::integer[], '{t.AccCursor}', '{t.AccTextSize}', '{t.AccContrast}', '{t.AccLanguage}')"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Updates an existing user entity in the database.
    /// </summary>
    /// <param name="t">The existing user entity to be updated.</param>
    /// <param name="newT">The new data for the user entity.</param>
    public void Update(User t, User newT) {
        string favorites = newT.Favorites != null ? string.Join(",", newT.Favorites) : string.Join(",", Array.Empty<int>());
        string statement = (
            $"UPDATE users SET " +
            $"name = '{newT.Name}', " +
            $"email = '{newT.Email}', " +
            $"favorites = ARRAY[{string.Join(",", favorites)}]::integer[], " +
            $"acc_cursor = '{newT.AccCursor}', " +
            $"acc_text_size = '{newT.AccTextSize}', " +
            $"acc_contrast = '{newT.AccContrast}', " +
            $"acc_language = '{newT.AccLanguage}' " +
            $"WHERE email = '{t.Email}'"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    /// <summary>
    /// Deletes a user entity from the database.
    /// </summary>
    /// <param name="t">The user entity to be deleted.</param>
    public void Delete(User t) {
        string statement = $"DELETE FROM users WHERE email = {t.Email}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
