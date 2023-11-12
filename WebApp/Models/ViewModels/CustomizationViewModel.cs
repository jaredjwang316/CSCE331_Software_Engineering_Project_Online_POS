using WebApp.Models.UnitOfWork;

namespace WebApp.Models.ViewModels;
public class CustomizationViewModel {
    public Product? SelectedProduct { get; set; }
    public List<SeriesInfo>? SeriesInformation { get; set; }
    public List<Product>? Products { get; set; }
}
