using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter the text to translate: ");
        string textToTranslate = Console.ReadLine();

        Console.Write("Enter the target language (e.g., 'fr' for French): ");
        string targetLanguage = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(textToTranslate) || string.IsNullOrWhiteSpace(targetLanguage))
        {
            Console.WriteLine("Both text and target language must be provided.");
            return;
        }

        GoogleCredential credentials = GoogleCredential.GetApplicationDefault();
        TranslationClient translationClient = TranslationClient.Create(credentials);

        try
        {
            // Translate the text
            TranslationResult result = await translationClient.TranslateTextAsync(textToTranslate, targetLanguage);

            // Display the translated text
            Console.WriteLine($"Original Text: {textToTranslate}");
            Console.WriteLine($"Translated Text: {result.TranslatedText}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Translation error: {ex.Message}");
        }
    }
}