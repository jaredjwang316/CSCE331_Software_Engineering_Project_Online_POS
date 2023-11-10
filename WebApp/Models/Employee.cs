/*
    File: Employee.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models;

/// <summary>
/// This class is used to represent a row in the Employee table.
/// </summary>
public class Employee {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public bool IsManager { get; set; }
    public string Email { get; set; }

    /// <summary>
    /// Constructor for the Employee class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="password"></param>
    /// <param name="isManager"></param>
    /// <param name="email"></param>
    public Employee(int id, string name, string password, bool isManager, string email) {
        Id = id;
        Name = name;
        Password = password;
        IsManager = isManager;
        Email = email;
    }
}
