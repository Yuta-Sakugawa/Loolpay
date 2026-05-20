using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Loolpay.Models;
using Loolpay.Data;
using Microsoft.EntityFrameworkCore;

namespace Loolpay.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Ensure some data exists if empty
        if (!_context.Users.Any())
        {
            _context.Users.AddRange(
                new User { Name = "Alice", Email = "alice@example.com" },
                new User { Name = "Bob", Email = "bob@example.com" }
            );
            await _context.SaveChangesAsync();
        }

        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
