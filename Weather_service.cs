using System;
using System.Net.Http;  //make HTTP requests
using System.Threading.Tasks;   //handle asynchronous tasks

class WeatherService
{
    private const string ApiKey = "1bac0154ece77068beb2a7b0772a710a"; // Jared's API key
    private const string BaseUrl = "https://home.openweathermap.org";

    public async Task GetWeatherDataAsync(string city)
    {
        using (var client = new HttpClient())
        {
            try
            {
                // Build the URL with the required parameters
                string url = $"{BaseUrl}?q={city}&appid={ApiKey}";

                // Send the HTTP GET request
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the JSON response
                    string json = await response.Content.ReadAsStringAsync();
                    WeatherData weatherData = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherData>(json);

                    // Display the weather data
                    Console.WriteLine($"Weather in {city}:");
                    Console.WriteLine($"Temperature: {weatherData.Main.Temperature}°C");
                    Console.WriteLine($"Humidity: {weatherData.Main.Humidity}%");
                    Console.WriteLine($"Description: {weatherData.Weather[0].Description}");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Sample Test Cases
        await RunSampleTestCases();

        // Additional application logic can go here...
    }

    static async Task RunSampleTestCases()
    {
        WeatherService weatherService = new WeatherService();

        // Sample Test Case 1: Successful Weather Retrieval
        await weatherService.GetWeatherDataAsync("New York");

        // Sample Test Case 2: Invalid City Name
        await weatherService.GetWeatherDataAsync("InvalidCityName");

        // Sample Test Case 3: Invalid API Key
        weatherService.SetApiKey("INVALID_API_KEY");
        await weatherService.GetWeatherDataAsync("New York");

        // Sample Test Case 4: Network Connectivity (You can disable network for this)
        // ...

        // Sample Test Case 5: Exception Handling
        // Modify the code to force an exception (e.g., provide an invalid URL)
        weatherService.SetApiUrl("invalid/url");
        await weatherService.GetWeatherDataAsync("New York");
    }
}