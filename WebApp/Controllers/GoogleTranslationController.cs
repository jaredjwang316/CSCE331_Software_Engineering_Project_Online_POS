using Microsoft.AspNetCore.Mvc;
public class GoogleTranslationController : Controller
{
    public async Task<IActionResult> Translate()
    {
        // Retrieve user input for text to translate and target language
        string textToTranslate = Request.Form["textToTranslate"]; // Replace with your actual input source
        string targetLanguage = Request.Form["targetLanguage"];   // Replace with your actual input source

        if (string.IsNullOrWhiteSpace(textToTranslate) || string.IsNullOrWhiteSpace(targetLanguage))
        {
            // Handle missing input, e.g., show an error message
            return View("Error");
        }

        var googleTranslationApi = new GoogleTranslationApi();

        // Perform the translation
        string translatedText = await googleTranslationApi.TranslateText(textToTranslate, targetLanguage);

        // Pass the translated text to the view
        ViewBag.OriginalText = textToTranslate;
        ViewBag.TranslatedText = translatedText;

        return View();
    }
}