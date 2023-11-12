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
    public string Name { get; set; }
    public string ImgUrl { get; set; }
    public bool MultiSelectable { get; set; }
    public bool IsProduct { get; set; }
    public bool IsCustomization { get; set; }
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
