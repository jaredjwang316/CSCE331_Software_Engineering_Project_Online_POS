using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;
using System;
using System.Threading.Tasks;

public class GoogleTranslationApi
{
    private readonly TranslationClient _translationClient;

    public GoogleTranslationApi()
    {
        GoogleCredential credentials = GoogleCredential.GetApplicationDefault();
        _translationClient = TranslationClient.Create(credentials);
    }

    public async Task<string> TranslateText(string textToTranslate, string targetLanguage)
    {
        try
        {
            TranslationResult result = await _translationClient.TranslateTextAsync(textToTranslate, targetLanguage);
            return result.TranslatedText;
        }
        catch (Exception ex)
        {
            // Handle translation error
            Console.WriteLine($"Translation error: {ex.Message}");
            return null; // or throw an exception
        }
    }
}