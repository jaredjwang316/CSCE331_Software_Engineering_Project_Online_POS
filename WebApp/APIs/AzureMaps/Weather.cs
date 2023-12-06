using System.Text.Json;
using WebApp.Models.AzureMaps.Weather;

namespace WebApp.APIs.AzureMaps;
/// <summary>
/// Provides methods to retrieve weather information from Azure Maps.
/// </summary>

/*
The Weather class serves as part of the WeatherService, authored for user accessibility within the Point of Sale (POS) system. 
This class integrates with Azure Maps API to retrieve weather-related information based on user location. The constructor initializes 
components, including the Azure Maps API key obtained from the configuration and an HttpContextAccessor for accessing HTTP context.

The GetCurrentCondition method asynchronously fetches the current weather conditions using latitude and longitude coordinates 
obtained from cookies in the HTTP request. It constructs a URL for the Azure Maps API weather endpoint and makes a request to retrieve 
weather data. Upon a successful response, the method parses the JSON response to extract temperature and weather icon code, creating a 
CurrentCondition object representing the current weather condition.

The GetWeatherIconName method maps the received weather icon code to an appropriate icon name for displaying weather icons in the POS system interface.
*/

public class Weather {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly string apiKey;
    private readonly string URL_BASE = "https://atlas.microsoft.com/weather";

    /// <summary>
    /// Initializes a new instance of the <see cref="Weather"/> class.
    /// </summary>
    public Weather() {
        apiKey = Config.AZURE_MAPS_API_KEY;
        httpContextAccessor = new HttpContextAccessor();
    }

    /// <summary>
    /// Gets the current weather condition based on the user's location.
    /// </summary>
    /// <returns>The current weather condition or null if the information is not available.</returns>
    public async Task<CurrentCondition?> GetCurrentCondition() {
        string latitude = httpContextAccessor.HttpContext!.Request.Cookies["latitude"] ?? "30";
        string longitude = httpContextAccessor.HttpContext!.Request.Cookies["longitude"] ?? "-90";
        
        string url = $"{URL_BASE}/currentConditions/json?subscription-key={apiKey}" +
                     $"&api-version=1.0&query={latitude},{longitude}&unit=imperial";
        
        JsonDocument? json = await MakeRequest(url);
        if (json == null) return null;

        double temperature;
        int iconCode;
        try {
            temperature = json.RootElement.GetProperty("results")[0].GetProperty("temperature").GetProperty("value").GetDouble();
            iconCode = json.RootElement.GetProperty("results")[0].GetProperty("iconCode").GetInt32();
        } catch {
            return null;
        }

        return new CurrentCondition(temperature, iconCode);
    }

    /// <summary>
    /// Makes an HTTP request to the specified URL and returns the JSON document.
    /// </summary>
    /// <param name="url">The URL to make the request to.</param>
    /// <param name="errorCallback">An optional callback to be executed in case of an error.</param>
    /// <returns>The JSON document or null if the request fails.</returns>
    static async Task<JsonDocument?> MakeRequest(string url, Action? errorCallback = null) {
        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) {
            errorCallback?.Invoke();
            return null;
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        JsonDocument json;
        try {
            json = JsonDocument.Parse(responseBody);
            return json;
        } catch {
            errorCallback?.Invoke();
        }

        return null;
    }

    /// <summary>
    /// Gets the weather icon name based on the provided icon code.
    /// </summary>
    /// <param name="iconCode">The icon code representing the weather condition.</param>
    /// <returns>The name of the weather icon or null if the icon code is not recognized.</returns>
    public string? GetWeatherIconName(int? iconCode) {
        string? iconName;
        iconName = iconCode switch
        {
            1 => "sunny",
            2 => "mostly_sunny",
            3 => "partly_sunny",
            4 => "intermittent_clouds",
            5 => "hazy_sunshine",
            6 => "mostly_cloudy",
            7 => "cloudy",
            8 => "dreary",
            11 => "fog",
            12 => "showers",
            13 => "mostly_cloudy_showers",
            14 => "partly_sunny_showers",
            15 => "thunderstorms",
            16 => "mostly_cloudy_thunderstorms",
            17 => "partly_sunny_thunderstorms",
            18 => "rain",
            19 => "flurries",
            20 => "mostly_cloudy_flurries",
            21 => "partly_sunny_flurries",
            22 => "snow",
            23 => "mostly_cloudy_snow",
            24 => "ice",
            25 => "sleet",
            26 => "freezing_rain",
            29 => "rain_and_snow",
            30 => "hot",
            31 => "cold",
            32 => "windy",
            33 => "clear",
            34 => "mostly_clear",
            35 => "partly_cloudy",
            36 => "intermittent_clouds_night",
            37 => "hazy_moonlight",
            38 => "mostly_cloudy_night",
            39 => "partly_cloudy_showers_night",
            40 => "mostly_cloudy_showers_night",
            41 => "partly_cloudy_thunderstorms_night",
            42 => "mostly_cloudy_thunderstorms_night",
            43 => "mostly_cloudy_flurries_night",
            44 => "mostly_cloudy_snow",
            _ => null,
        };

        return iconName;
    }
}