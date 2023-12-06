using WebApp.Models.UnitOfWork;

namespace WebApp.Models.ViewModels;

/// <summary>
/// Represents a view model for customization-related views.
/// </summary>
public class CustomizationViewModel {
    /// <summary>
    /// Gets or sets the selected product for customization.
    /// </summary>
    public Product? SelectedProduct { get; set; }
    
    /// <summary>
    /// Gets or sets the list of series information for customization.
    /// </summary>
    public List<SeriesInfo>? SeriesInformation { get; set; }
    
    /// <summary>
    /// Gets or sets the list of products for customization.
    /// </summary>
    public List<Product>? Products { get; set; }
}
