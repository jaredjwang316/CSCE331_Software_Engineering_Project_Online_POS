
namespace WebApp.Models.JsonModels;
public class AddItemModel {
    public int ProductId { get; set; }
    public List<int> CustomizationIds { get; set; } = new();
    public int Quantity { get; set; }
}
