/*
    File: UserDao.cs
    Author: Griffin Beaudreau
    Date: November 29, 2023
*/

using WebApp.Models.UnitOfWork;

namespace WebApp.Data;

public class UserDao : IDao<User> {
    private readonly CommandHandler commandHandler;

    public UserDao(CommandHandler commandHandler) {
        this.commandHandler = commandHandler;
    }

    public User Get(int id) {
        throw new NotSupportedException();
    }

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

    public void Add(User t) {
        string favorites = t.Favorites != null ? string.Join(",", t.Favorites) : string.Join(",", Array.Empty<int>());
        string statement = (
            $"INSERT INTO users (name, email, favorites, acc_cursor, acc_text_size, acc_contrast, acc_language) " +
            $"VALUES ('{t.Name}', '{t.Email}', ARRAY[{favorites}]::integer[], '{t.AccCursor}', '{t.AccTextSize}', '{t.AccContrast}', '{t.AccLanguage}')"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

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

    public void Delete(User t) {
        string statement = $"DELETE FROM users WHERE email = {t.Email}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
