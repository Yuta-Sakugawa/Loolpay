using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Loolpay.Data;
using Loolpay.Models;
using Microsoft.AspNetCore.Identity;

namespace Loolpay.Controllers;

public class PlacesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlacesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
        _userManager = userManager;
    }

    // GET: Places
    public async Task<IActionResult> Index(string? searchString, StoreGenre? genre, string? payment)
    {
        var stores = from s in _context.Stores
                    select s;

        if (!string.IsNullOrEmpty(searchString))
        {
            stores = stores.Where(s => s.StoreName.Contains(searchString) 
                                    || (s.StoreAddress != null && s.StoreAddress.Contains(searchString)));
        }

        if (genre.HasValue)
        {
            stores = stores.Where(s => s.Genre == genre.Value);
        }

        if (!string.IsNullOrEmpty(payment))
        {
            // LIKE句を使ってDB側で検索します。
            // 前後にカンマやスペースがあってもマッチするようにパターンを作成します。
            // データの形式が "Cash, Credit Card, QR Payment" のため、
            // 「%Cash%」というLIKEで検索します。
            // 誤判定を防ぐため、完全一致するアイテムをLIKEで囲む工夫が必要です。
            // カンマ区切りのため、以下のようにパターンマッチを行います。
            // 1. 先頭: payment + ","
            // 2. 中間: ", " + payment + ","
            // 3. 末尾: ", " + payment
            // 4. 単一: payment
            
            // シンプルかつ有効なアプローチとして、LIKE '%payment%' でのフィルタリングを
            // 改良し、区切り文字を考慮した精度の高いマッチングにします。
            
            stores = stores.Where(s => s.Pay != null && 
                                      (s.Pay == payment || 
                                       s.Pay.StartsWith(payment + ",") || 
                                       s.Pay.Contains(", " + payment + ",") || 
                                       s.Pay.EndsWith(", " + payment)));
        }

        ViewData["CurrentSearchString"] = searchString;
        ViewData["CurrentGenre"] = genre;
        ViewData["CurrentPayment"] = payment;
        
        return View(await stores.ToListAsync());
    }

    // GET: Places/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var store = await _context.Stores
            .FirstOrDefaultAsync(m => m.StoreId == id);
        if (store == null)
        {
            return NotFound();
        }

        // ログの記録
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            var log = new StoreViewLog
            {
                UserId = userId,
                StoreId = store.StoreId,
                ViewedAt = DateTime.Now
            };
            _context.StoreViewLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        return View(store);
    }

    // GET: Places/Create
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Places/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([Bind("StoreName,StoreAddress,SelectedPaymentMethods,Genre")] Store store, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (store.SelectedPaymentMethods != null && store.SelectedPaymentMethods.Any())
            {
                store.Pay = string.Join(", ", store.SelectedPaymentMethods);
            }

            if (imageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string path = Path.Combine(wwwRootPath + "/images/stores/", fileName);

                var directory = Path.Combine(wwwRootPath, "images/stores");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                store.ImagePath = fileName;
            }

            store.LastUpdated = DateTime.Now;

            _context.Add(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(store);
    }

    // GET: Places/Edit/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var store = await _context.Stores.FindAsync(id);
        if (store == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(store.Pay))
        {
            store.SelectedPaymentMethods = store.Pay.Split(", ").ToList();
        }

        return View(store);
    }

    // POST: Places/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int id, [Bind("StoreId,StoreName,StoreAddress,SelectedPaymentMethods,ImagePath,Genre")] Store store, IFormFile? imageFile)
    {
        if (id != store.StoreId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (store.SelectedPaymentMethods != null && store.SelectedPaymentMethods.Any())
                {
                    store.Pay = string.Join(", ", store.SelectedPaymentMethods);
                }
                else
                {
                    store.Pay = null;
                }

                if (imageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string path = Path.Combine(wwwRootPath + "/images/stores/", fileName);

                    var directory = Path.Combine(wwwRootPath, "images/stores");
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(store.ImagePath))
                    {
                        var oldPath = Path.Combine(wwwRootPath + "/images/stores/", store.ImagePath);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    store.ImagePath = fileName;
                }

                store.LastUpdated = DateTime.Now;

                _context.Update(store);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(store.StoreId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(store);
    }

    // GET: Places/Delete/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var store = await _context.Stores
            .FirstOrDefaultAsync(m => m.StoreId == id);
        if (store == null)
        {
            return NotFound();
        }

        return View(store);
    }

    // POST: Places/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            _context.Stores.Remove(store);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool StoreExists(int id)
    {
        return _context.Stores.Any(e => e.StoreId == id);
    }
}
