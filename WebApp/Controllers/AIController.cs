
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using WebApp.AI;
using WebApp.APIs.GoogleTranslate;


namespace WebApp.Controllers;

public class AIController : Controller
{
    private readonly ILogger<AIController> _logger;

    public AIController(ILogger<AIController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> TTS(string text) {
        var tts = new TTS();
        await tts.Run(text);

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> GetResponse(string input) {
        string response = "";
        string history = GetHistory();

        string systemPrompt = "!RespondOnlyInEnglish!ExpertONGongCha!IF!QuestiUnrelaTOGongChaORGreetSAY\"I'mSorry,MyKnowledgeIsLimitedToGongCha\"!GreetOk!";

        try {
            response = await new Chatbot("SYSPROMPT:" + systemPrompt + "\n\n" + history).Run(input);
        } catch (Exception e) {
            return BadRequest(e.Message);
        }

        bool TTS = HttpContext.Request.Cookies["tts"] == "true";
        if (TTS) {
            var ignore = new TTS().Run(response).ContinueWith(task => {
                if (task.IsFaulted) {
                    Console.WriteLine($"Error: {task.Exception?.InnerException?.Message ?? "No details available"}");
                }
            });
        }

        string translatedResponse = await new GoogleTranslate().TranslateText(response);

        SetHistory(history + $"\nUser: {input}\nAssistant: {response}");
        return Ok(translatedResponse);
    }

    public string GetHistory() {
        string? history;
        history = HttpContext.Session.GetString("chatbotHistory") ?? "";

        return history;
    }
    public IActionResult GetHistorySplit() {
        string? history = GetHistory();
        string[] splitHistory = history.Split("\n");

        if (splitHistory.Length > 5) {
            splitHistory = splitHistory[^5..];
            SetHistory(string.Join("\n", splitHistory));
        }

        var translate = new GoogleTranslate();

        List<string> translatedHistory = new();
        foreach(string message in splitHistory) {
            if (message.Contains("User:") || message.Contains("Assistant:")) {
                string type = message.Split(":")[0];
                string text = message.Split(":")[1];
                text = translate.TranslateText(text).Result;

                if (type == "User") {
                    translatedHistory.Add($"User: {text}");
                } else {
                    translatedHistory.Add($"Assistant: {text}");
                }
            }
        }

        return Ok(translatedHistory);
    }
    public void SetHistory(string history) {
        HttpContext.Session.SetString("chatbotHistory", history);
    }

    public IActionResult ClearHistory() {
        HttpContext.Session.SetString("chatbotHistory", "");
        return Ok();
    }
}
