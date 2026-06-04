using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Loolpay.Data;

namespace Loolpay.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Logs()
    {
        var logs = await _context.StoreViewLogs
            .Include(l => l.Store)
            .Include(l => l.User)
            .OrderByDescending(l => l.ViewedAt)
            .ToListAsync();
        return View(logs);
    }
    
    public async Task<IActionResult> Rankings()
    {
        var rankings = await _context.StoreViewLogs
            .GroupBy(l => l.StoreId)
            .Select(g => new { 
                StoreId = g.Key, 
                StoreName = g.FirstOrDefault()!.Store!.StoreName,
                ViewCount = g.Count() 
            })
            .OrderByDescending(r => r.ViewCount)
            .ToListAsync();
        
        ViewBag.Rankings = rankings;
        return View();
    }
}
