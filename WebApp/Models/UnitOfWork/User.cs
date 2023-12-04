/*
    File: User.cs
    Author: Griffin Beaudreau
    Date: November 29, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// Represents information about a user in the application.
/// </summary>
public class User {
    /// <summary>
    /// Gets or sets the name of the user.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; }
    
    /// <summary>
    /// Gets or sets the array of user favorites.
    /// </summary>
    public int[] Favorites { get; set; }
    
    /// <summary>
    /// Gets or sets the accessibility cursor setting for the user.
    /// </summary>
    public string AccCursor { get; set; }
    
    /// <summary>
    /// Gets or sets the accessibility text size setting for the user.
    /// </summary>
    public string AccTextSize { get; set; }
    
    /// <summary>
    /// Gets or sets the accessibility contrast setting for the user.
    /// </summary>
    public string AccContrast { get; set; }
    
    /// <summary>
    /// Gets or sets the accessibility language setting for the user.
    /// </summary>
    public string AccLanguage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="email">The email address of the user.</param>
    /// <param name="favorites">The array of user favorites.</param>
    /// <param name="accCursor">The accessibility cursor setting for the user.</param>
    /// <param name="accTextSize">The accessibility text size setting for the user.</param>
    /// <param name="accContrast">The accessibility contrast setting for the user.</param>
    /// <param name="accLanguage">The accessibility language setting for the user.</param>
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
