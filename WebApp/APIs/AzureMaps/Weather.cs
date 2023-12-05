/*
File: APIs/AzureMaps/Weather.cs
Author: Jared Wang and Griffin Beaudreau
Date: December 4th, 2023
Purpose: The Weather class serves as part of the WeatherService, authored for user accessibility within the Point of Sale (POS) system. 
This class integrates with Azure Maps API to retrieve weather-related information based on user location. The constructor initializes 
components, including the Azure Maps API key obtained from the configuration and an HttpContextAccessor for accessing HTTP context.
*/
using System.Text.Json;
using WebApp.Models.AzureMaps.Weather;

namespace WebApp.APIs.AzureMaps;


public class Weather {
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly string apiKey;
    private readonly string URL_BASE = "https://atlas.microsoft.com/weather";

    public Weather() {
        apiKey = Config.AZURE_MAPS_API_KEY;
        httpContextAccessor = new HttpContextAccessor();
    }

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