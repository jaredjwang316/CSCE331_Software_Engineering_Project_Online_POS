using WebApp.Models.Cart;
using WebApp.Models.UnitOfWork;

namespace WebApp.Models.ViewModels;
public class EditViewModel {
    public Cart.Cart? Cart { get; set; }
    public int Index { get; set; }
    public List<SeriesInfo>? SeriesInformation { get; set; }
    public List<Product>? Products { get; set; }
}
