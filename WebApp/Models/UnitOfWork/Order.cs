/*
    File: Order.cs
    Author: Griffin Beaudreau
    Date: October 23, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the Order table.
/// </summary>
public class Order {
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the employee associated with the order.
    /// </summary>
    public int EmployeeId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the customer placing the order.
    /// </summary>
    public string CustomerName { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the order was placed.
    /// </summary>
    public DateTime OrderDate { get; set; }
    
    /// <summary>
    /// Gets or sets the total price of the order.
    /// </summary>
    public double TotalPrice { get; set; }
    
    /// <summary>
    /// Gets or sets the list of item identifiers associated with the order.
    /// </summary>
    public List<int> ItemIds { get; set; }

    /// <summary>
    /// Constructor for the Order class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="employeeId"></param>
    /// <param name="customerName"></param>
    /// <param name="orderDate"></param>
    /// <param name="totalPrice"></param>
    /// <param name="itemIds"></param>
    public Order(int id, int employeeId, String customerName, DateTime orderDate, double totalPrice, List<int> itemIds) {
        Id = id;
        EmployeeId = employeeId;
        CustomerName = customerName;
        OrderDate = orderDate;
        TotalPrice = totalPrice;
        ItemIds = itemIds;
    }
}
