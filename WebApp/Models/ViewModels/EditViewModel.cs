using WebApp.Models.Cart;
using WebApp.Models.UnitOfWork;

namespace WebApp.Models.ViewModels;

/// <summary>
/// Represents a view model for editing cart-related views.
/// </summary>
public class EditViewModel {
    /// <summary>
    /// Gets or sets the cart to be edited.
    /// </summary>
    public Cart.Cart? Cart { get; set; }
    
    /// <summary>
    /// Gets or sets the index of the cart item to be edited.
    /// </summary>
    public int Index { get; set; }
    
    /// <summary>
    /// Gets or sets the list of series information for customization.
    /// </summary>
    public List<SeriesInfo>? SeriesInformation { get; set; }
    
    /// <summary>
    /// Gets or sets the list of products for customization.
    /// </summary>
    public List<Product>? Products { get; set; }
}
