/*
    File: SeriesInfo.cs
    Author: Griffin Beaudreau
    Date: October 31, 2023
*/

namespace WebApp.Models;

/// <summary>
/// This class is used to represent a row in the series_info table.
/// </summary>
public class SeriesInfo {
    public string Name { get; set; }
    public string ImgUrl { get; set; }
    public bool MultiSelectable { get; set; }

    /// <summary>
    /// Constructor for the SeriesInfo class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="img_url"></param>
    public SeriesInfo(string name, string img_url, bool multi_selectable) {
        Name = name;
        ImgUrl = img_url;
        MultiSelectable = multi_selectable;
    }
}
