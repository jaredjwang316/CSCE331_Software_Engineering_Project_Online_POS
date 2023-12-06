
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using WebApp.AI;
using WebApp.APIs.GoogleTranslate;


namespace WebApp.Controllers;

[ApiController]
public class AIController : Controller
{
    private readonly ILogger<AIController> _logger;
    private readonly int MAX_HISTORY_LENGTH = 10;

    public AIController(ILogger<AIController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Queries the AI for a response to the given input.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost, Route("AI/GetResponse/{input}")]
    public async Task<IActionResult> GetResponse(string input) {
        string response = "";
        string history = GetHistory();

        string systemPrompt = "";

        try {
            var chatbot = new Chatbot(systemPrompt, 300, 0.9f);
            chatbot.AddHistory(history);
            response = await chatbot.Run(input);
        } catch (Exception e) {
            return BadRequest(e.Message);
        }

        string translatedResponse = await new GoogleTranslate().TranslateText(response);
        SetHistory(history + $"\nUser: {input}\nAssistant: {response}");
        return Ok(translatedResponse);
    }

    /// <summary>
    /// Gets the chatbot history from the session.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("AI/GetHistory")]
    public string GetHistory() {
        string? history;
        history = HttpContext.Session.GetString("chatbotHistory") ?? "";

        return history;
    }

    /// <summary>
    /// Gets the chatbot history from the session and splits it into an array.
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("AI/GetHistorySplit")]
    public IActionResult GetHistorySplit() {
        string? history = GetHistory();
        string[] splitHistory = history.Split("\n");

        if (splitHistory.Length > MAX_HISTORY_LENGTH) {
            splitHistory = splitHistory[^MAX_HISTORY_LENGTH..];
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

    /// <summary>
    /// Sets the chatbot history in the session.
    /// </summary>
    /// <param name="history"></param>
    [HttpPost, Route("AI/SetHistory")]
    public void SetHistory(string history) {
        HttpContext.Session.SetString("chatbotHistory", history);
    }

    /// <summary>
    /// Clears the chatbot history in the session.
    /// </summary>
    /// <returns></returns>
    [HttpDelete, Route("AI/ClearHistory")]
    public IActionResult ClearHistory() {
        HttpContext.Session.SetString("chatbotHistory", "");
        return Ok();
    }
}
