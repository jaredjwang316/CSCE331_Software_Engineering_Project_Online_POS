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
            (int[])reader.GetValue(2)
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
                (int[])reader.GetValue(2)
            ));
        }

        reader?.Close();
        return users;
    }

    public void Add(User t) {
        string favorites = t.Favorites != null ? string.Join(",", t.Favorites) : string.Join(",", Array.Empty<int>());
        string statement = (
            $"INSERT INTO users (name, email, favorites) " +
            $"VALUES ('{t.Name}', '{t.Email}', ARRAY[{favorites}]::integer[])"
        );
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Update(User t, User newT) {
        string favorites = newT.Favorites != null ? string.Join(",", newT.Favorites) : string.Join(",", Array.Empty<int>());
        string statement = (
            $"UPDATE users SET " +
            $"name = '{newT.Name}', " +
            $"email = '{newT.Email}', " +
            $"favorites = ARRAY[{string.Join(",", favorites)}]::integer[] " +
            $"WHERE email = '{t.Email}'"
        );
        
        commandHandler.ExecuteNonQuery(statement);
    }

    public void Delete(User t) {
        string statement = $"DELETE FROM users WHERE email = {t.Email}";
        commandHandler.ExecuteNonQuery(statement);
    }
}
