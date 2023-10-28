using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WeatherService
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public WeatherService(string apiKey)
    {
        this.apiKey = apiKey;
        this.httpClient = new HttpClient();
    }

    public async Task<string> GetWeatherAsync(string city)
    {
        string apiUrl = "https://openweathermap.org/";
        
        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                // You can parse the JSON response here and extract the weather data.
                return json;
            }
            else
            {
                throw new Exception("Failed to fetch weather data.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Weather API request failed: {ex.Message}");
        }
    }
}