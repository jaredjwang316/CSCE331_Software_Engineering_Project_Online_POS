using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

using Npgsql;
using WebApp.Data;
using System.Reflection.Metadata.Ecma335;

namespace WebApp.Controllers;

public class ManagerController : Controller
{
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(ILogger<ManagerController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
