

namespace WebApp.Models.AzureMaps.Weather;
/// <summary>
/// Represents the current weather condition, including temperature and icon code.
/// </summary>
public class CurrentCondition {
    /// <summary>
    /// Gets or sets the current temperature.
    /// </summary>
    public double? Temperature { get; set; }
    
    /// <summary>
    /// Gets or sets the icon code representing the weather condition.
    /// </summary>
    public int? IconCode { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurrentCondition"/> class.
    /// </summary>
    /// <param name="temperature">The current temperature.</param>
    /// <param name="iconCode">The icon code representing the weather condition.</param>
    public CurrentCondition(double? temperature, int? iconCode) {
        Temperature = temperature;
        IconCode = iconCode;
    }
}
