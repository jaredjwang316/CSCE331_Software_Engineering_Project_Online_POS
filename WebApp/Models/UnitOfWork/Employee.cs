/*
    File: Employee.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the Employee table.
/// </summary>
public class Employee {
    /// <summary>
    /// Gets or sets the unique identifier for the employee.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the employee.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the password associated with the employee's account.
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the employee is a manager.
    /// </summary>
    public bool IsManager { get; set; }
    
    /// <summary>
    /// Gets or sets the email address of the employee.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Employee"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the employee.</param>
    /// <param name="name">The name of the employee.</param>
    /// <param name="password">The password associated with the employee's account.</param>
    /// <param name="isManager">A value indicating whether the employee is a manager.</param>
    /// <param name="email">The email address of the employee.</param>
    
    public Employee(int id, string name, string password, bool isManager, string email) {
        Id = id;
        Name = name;
        Password = password;
        IsManager = isManager;
        Email = email;
    }
}
