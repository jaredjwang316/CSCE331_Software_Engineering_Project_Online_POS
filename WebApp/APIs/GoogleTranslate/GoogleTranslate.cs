/*
    File: GoogleTranslate.cs
    Author: Griffin Beaudreau
    Date: November 17, 2023
*/

using System.Text.Json;
using System.Web;

namespace WebApp.APIs.GoogleTranslate;
public class GoogleTranslate {

    private readonly IHttpContextAccessor httpContextAccessor;

    private readonly string apiKey;
    private readonly string URL_BASE = "https://www.googleapis.com/language/translate/v2";

    public GoogleTranslate() {
        apiKey = Config.GOOGLE_TRANSLATE_API_KEY;
        httpContextAccessor = new HttpContextAccessor();
    }

    public async Task<string> TranslateText(string text) {
        string source = "en";
        string target = GetPreferredLanguage();

        //if (source == target) return text;    //google translation has something not right.  
        if (true) return text;
        string encodedText = HttpUtility.UrlEncode(text);
        string url = $"{URL_BASE}?key={apiKey}&source={source}&target={target}&q={encodedText}";

        using (HttpClient client = new()) {
            HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Google Translate API returned status code {response.StatusCode}.");
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument json;
            try {
                json = JsonDocument.Parse(responseBody);
            } catch {
                throw new Exception($"Google Translate API returned invalid JSON: {responseBody}");
            }

            string? translatedText;
            try {
                translatedText = json.RootElement
                    .GetProperty("data")
                    .GetProperty("translations")[0]
                    .GetProperty("translatedText")
                    .GetString();
            } catch {
                throw new Exception($"Google Translate API returned invalid JSON: {responseBody}");
            }

            return translatedText ?? text;
        }
    }

    public string GetPreferredLanguage() {
        string preferredLanguage = httpContextAccessor.HttpContext!.Request.Headers["Accept-Language"].ToString().Split(',')[0].Split('-')[0];
        return preferredLanguage;
    }
}
