/*
    File: User.cs
    Author: Griffin Beaudreau
    Date: November 29, 2023
*/

namespace WebApp.Models.UnitOfWork;

public class User {
    public string Name { get; set; }
    public string Email { get; set; }
    public int[] Favorites { get; set; }

    public User(string name, string email, int[] favorites) {
        Name = name;
        Email = email;
        Favorites = favorites;
    }
}
