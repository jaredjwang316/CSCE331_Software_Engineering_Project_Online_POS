

namespace WebApp.Models.AzureMaps.Weather;
public class CurrentCondition {
    public double? Temperature { get; set; }
    public int? IconCode { get; set; }

    public CurrentCondition(double? temperature, int? iconCode) {
        Temperature = temperature;
        IconCode = iconCode;
    }
}
