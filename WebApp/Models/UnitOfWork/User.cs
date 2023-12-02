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
    public string AccCursor { get; set; }
    public string AccTextSize { get; set; }
    public string AccContrast { get; set; }
    public string AccLanguage { get; set; }

    public User(
        string name,
        string email,
        int[] favorites,
        string accCursor = "false",
        string accTextSize = "false",
        string accContrast = "normal",
        string accLanguage = "")
    {
        Name = name;
        Email = email;
        Favorites = favorites;
        AccCursor = accCursor;
        AccTextSize = accTextSize;
        AccContrast = accContrast;
        AccLanguage = accLanguage;
    }
}
