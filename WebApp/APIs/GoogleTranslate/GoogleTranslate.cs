/*
    File: APIs/GoogleTranslate/GoogleTranslate.cs
    Author: Griffin Beaudreau
    Date: November 17, 2023
    Purpose: The GoogleTranslate class facilitates language translation functionalities within the Point of Sale (POS) system. 
    This class integrates with the Google Translate API to dynamically translate  text based on the user's preferred language. 
    The constructor initializes the necessary components, including an  API key retrieved from the configuration and an 
    HttpContextAccessor for accessing HTTP context.
    The TranslateText method asynchronously translates input text from a source language (defaulting to English 'en') to 
    the user's preferred language. It constructs a URL using the Google Translate API endpoint, API key, source, target 
    languages, and the provided text. Upon receiving a successful response, the method parses the JSON response to extract 
    the translated text, providing a fallback to the original text if translation fails or no translation is available.

    The GetPreferredLanguage method retrieves the user's preferred language from the Accept-Language header in the HTTP request, 
    considering the primary language specified in the header.
*/

using System.Text.Json;
using System.Web;
using WebApp.Data;
using WebApp.Models.UnitOfWork;
using System.Security.Claims;


namespace WebApp.APIs.GoogleTranslate;

/// <summary>
/// Provides translation services using the Google Translate API.
/// </summary>
public class GoogleTranslate {
    /// <summary>
    /// Gets or sets the current language for translation.
    /// </summary>
    public string CurrentLanguage = "en";
    
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly string apiKey;
    private readonly string URL_BASE = "https://www.googleapis.com/language/translate/v2";

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleTranslate"/> class.
    /// </summary>
    public GoogleTranslate() {
        apiKey = Config.GOOGLE_TRANSLATE_API_KEY;
        httpContextAccessor = new HttpContextAccessor();

        CurrentLanguage = httpContextAccessor.HttpContext!.Request.Cookies["CurrentLanguage"]
            ?? GetPreferredLanguage();
    }

    /// <summary>
    /// Translates the specified text to the current language.
    /// </summary>
    /// <param name="text">The text to be translated.</param>
    /// <returns>The translated text or the original text if translation fails.</returns>
    public async Task<string> TranslateText(string text) {
        return await Translate(text);
    }
    public async Task<string> Translate(string text) {
        string source = "en";
        if (source == CurrentLanguage) return text;

        string encodedText = HttpUtility.UrlEncode(text);
        string url = $"{URL_BASE}?key={apiKey}&source={source}&target={CurrentLanguage}&q={encodedText}";

        using HttpClient client = new();

        for (int i = 0; i < 3; i++) {
            HttpResponseMessage? response;
            try {
                response = await client.GetAsync(url);
            } catch  {
                // Retry
                await Task.Delay(2000);
                continue;
            }

            if (response is null || !response.IsSuccessStatusCode) {
                break;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument json;
            try {
                json = JsonDocument.Parse(responseBody);
            }
            catch {
                break;
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

        return text;
    }

    /// <summary>
    /// Translates a collection of texts concurrently.
    /// </summary>
    /// <param name="texts"></param>
    /// <returns>
    /// A queue of translated texts. The queue contains untranslated texts if translation fails.
    /// </returns>
    public async Task<Queue<string>> Translate(IEnumerable<string> texts) {
        var translationTasks = texts.Select(Translate);
        var translations = await Task.WhenAll(translationTasks);
        return new Queue<string>(translations);
    }

    public async Task<Dictionary<string, string>> TranslateAsDict(IEnumerable<string> texts) {
        var translationTasks = texts.Select(Translate);
        var translations = await Task.WhenAll(translationTasks);
        return texts.Zip(translations, (text, translation) => new { text, translation })
            .ToDictionary(x => x.text, x => x.translation);
    }

    /// <summary>
    /// Gets the preferred language from the request headers.
    /// </summary>
    /// <returns>The preferred language or "en" if not available.</returns>
    public string GetPreferredLanguage() {
        string? preferredLanguage = httpContextAccessor.HttpContext!.Request.Headers["Accept-Language"].ToString().Split(',')[0].Split('-')[0];
        preferredLanguage ??= "en";
        return preferredLanguage;
    }

    /// <summary>
    /// Gets the list of supported languages and their codes.
    /// </summary>
    /// <returns>An array of key-value pairs representing supported languages and their codes.</returns>
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
