/*
    File: APIs/GoogleTranslate/GoogleTranslate.cs
    Author: Griffin Beaudreau
    Date: November 17, 2023
*/

using System.Text.Json;
using System.Web;
using WebApp.Data;
using WebApp.Models.UnitOfWork;
using System.Security.Claims;


namespace WebApp.APIs.GoogleTranslate;
public class GoogleTranslate {
    public string CurrentLanguage = "en";
    
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly string apiKey;
    private readonly string URL_BASE = "https://www.googleapis.com/language/translate/v2";

    public GoogleTranslate() {
        apiKey = Config.GOOGLE_TRANSLATE_API_KEY;
        httpContextAccessor = new HttpContextAccessor();

        CurrentLanguage = httpContextAccessor.HttpContext!.Request.Cookies["CurrentLanguage"]
            ?? GetPreferredLanguage();
    }

    public async Task<string> TranslateText(string text) {
        string source = "en";
        if (source == CurrentLanguage) return text;

        string encodedText = HttpUtility.UrlEncode(text);
        string url = $"{URL_BASE}?key={apiKey}&source={source}&target={CurrentLanguage}&q={encodedText}";

        using HttpClient client = new();
        HttpResponseMessage response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return text;
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        JsonDocument json;
        try {
            json = JsonDocument.Parse(responseBody);
        }
        catch {
            return text;
        }

        string? translatedText;
        try {
            translatedText = json.RootElement
                .GetProperty("data")
                .GetProperty("translations")[0]
                .GetProperty("translatedText")
                .GetString();
        }
        catch {
            translatedText = null;
        }

        return translatedText ?? text;
    }

    public string GetPreferredLanguage() {
        string? preferredLanguage = httpContextAccessor.HttpContext!.Request.Headers["Accept-Language"].ToString().Split(',')[0].Split('-')[0];
        preferredLanguage ??= "en";
        return preferredLanguage;
    }

    public KeyValuePair<string, string>[]? GetSupportedLanguages() {
        string url = $"{URL_BASE}/languages?target=en&key={apiKey}";
        using HttpClient client = new();
        HttpResponseMessage response = client.GetAsync(url).Result;
        if (!response.IsSuccessStatusCode)
        {
            KeyValuePair<string, string>[]? empty = Array.Empty<KeyValuePair<string, string>>();
            return empty;
        }

        string responseBody = response.Content.ReadAsStringAsync().Result;
        JsonDocument json;
        KeyValuePair<string, string>[]? languages;
        try {
            json = JsonDocument.Parse(responseBody);

            languages = json.RootElement
                .GetProperty("data")
                .GetProperty("languages")
                .EnumerateArray()
                .Select(x => {
                    string code = x.GetProperty("language").GetString()!.Split("-")[0];
                    string name = x.GetProperty("name").GetString()!;
                    return new KeyValuePair<string, string>(code, name);
                })
                .ToArray();
        }
        catch {
            languages = null;
        }

        return languages;
    }
}
