
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using WebApp.AI;


namespace WebApp.Controllers;

public class AIController : Controller
{
    private readonly ILogger<AIController> _logger;

    public AIController(ILogger<AIController> logger)
    {
        _logger = logger;
    }

    public async void TTS(string text) {
        var tts = new TTS();
        await tts.Run(text);
    }
}
