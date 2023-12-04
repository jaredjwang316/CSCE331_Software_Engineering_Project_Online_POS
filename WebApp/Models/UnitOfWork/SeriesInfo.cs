/*
    File: SeriesInfo.cs
    Author: Griffin Beaudreau
    Date: October 31, 2023
*/

namespace WebApp.Models.UnitOfWork;

/// <summary>
/// This class is used to represent a row in the series_info table.
/// </summary>
public class SeriesInfo {
    /// <summary>
    /// Gets or sets the name of the series.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the URL of the image associated with the series.
    /// </summary>
    public string ImgUrl { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the series is multi-selectable.
    /// </summary>
    public bool MultiSelectable { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the series is considered a product.
    /// </summary>
    public bool IsProduct { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the series is a customization.
    /// </summary>
    public bool IsCustomization { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether the series is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Constructor for the SeriesInfo class.
    /// </summary>
    /// <param name="name">The name of the series.</param>
    /// <param name="img_url">The URL of the image associated with the series.</param>
    /// <param name="multi_selectable">Whether or not the series is multi-selectable.</param>
    /// <param name="is_product">Whether or not the series is a product.</param>
    /// <param name="is_customization">Whether or not the series is a customization.</param>
    /// <param name="is_hidden">Whether or not the series is hidden.</param>
    /// <returns>A SeriesInfo object.</returns>
    public SeriesInfo(
        string name,
        string img_url,
        bool multi_selectable,
        bool is_product,
        bool is_customization,
        bool is_hidden
    ) {
        Name = name;
        ImgUrl = img_url;
        MultiSelectable = multi_selectable;
        IsProduct = is_product;
        IsCustomization = is_customization;
        IsHidden = is_hidden;
    }
}
