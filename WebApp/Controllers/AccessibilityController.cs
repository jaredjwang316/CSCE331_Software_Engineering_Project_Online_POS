using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class AccessibilityController : Controller {
    private readonly ILogger<AccessibilityController> _logger;

    public AccessibilityController(ILogger<AccessibilityController> logger)
    {
        _logger = logger;
    }

    public void ToggleAccessibilityMenu() {
        Console.WriteLine("Toggling accessibility menu.");
    }
}
