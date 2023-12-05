/* 
    File: Controllers/StoryController.cs
    Author: Nihar Shah
    Date: December 3, 2023
    Purpose: The StoryController.cs file is part of the backend structure for the Gong Cha restaurant's web application. 
    It consists of controller logic responsible for handling requests related to displaying the story section and 
    managing user redirection based on roles within the Point of Sale (POS) system.  
    This file manages the actions and routing within the Gong Cha POS system. It includes various 
    methods to handle different functionalities, such as rendering the story page, managing user redirection based 
    on roles (Cashier, Manager), retrieving preferred language settings, and handling errors.
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.UnitOfWork;
using WebApp.Models;
using WebApp.Data;

using Google.Cloud.Translation.V2;
using System.Text.Json;

using WebApp.APIs.AzureMaps;
using WebApp.Models.AzureMaps.Weather;
using WebApp.APIs.GoogleTranslate;

namespace WebApp.Controllers;

public class StoryController : Controller
{
    private readonly ILogger<StoryController> _logger;

    public StoryController(ILogger<StoryController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(bool autoRedirect = true)
    {
        if (autoRedirect && User.Identity!.IsAuthenticated && User.IsInRole("Cashier")) {
            return RedirectToAction("Index", "Cashier");
        } else if (autoRedirect && User.Identity!.IsAuthenticated && User.IsInRole("Manager")) {
            return RedirectToAction("Index", "Manager");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public string GetPreferredLanguage()
    {
        var preferredLanguage = HttpContext.Request.Headers["Accept-Language"].ToString().Split(',')[0];
        return preferredLanguage;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
