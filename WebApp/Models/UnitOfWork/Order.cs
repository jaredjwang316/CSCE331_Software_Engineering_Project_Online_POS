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
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public double TotalPrice { get; set; }
    public List<int> ItemIds { get; set; }

    /// <summary>
    /// Constructor for the Order class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="employeeId"></param>
    /// <param name="customerName"></param>
    /// <param name="orderDate"></param>
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
