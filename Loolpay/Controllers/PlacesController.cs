using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Loolpay.Data;
using Loolpay.Models;

namespace Loolpay.Controllers;

public class PlacesController : Controller
{
    private readonly ApplicationDbContext _context;

    public PlacesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Places
    public async Task<IActionResult> Index(string searchString)
    {
        var stores = from s in _context.Stores
                    select s;

        if (!string.IsNullOrEmpty(searchString))
        {
            stores = stores.Where(s => s.StoreName.Contains(searchString) 
                                    || (s.StoreAddress != null && s.StoreAddress.Contains(searchString)));
        }

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

        return View(store);
    }

    // GET: Places/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Places/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("StoreName,StoreAddress,SelectedPaymentMethods")] Store store)
    {
        if (ModelState.IsValid)
        {
            if (store.SelectedPaymentMethods != null && store.SelectedPaymentMethods.Any())
            {
                store.Pay = string.Join(", ", store.SelectedPaymentMethods);
            }
            _context.Add(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(store);
    }

    // GET: Places/Edit/5
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
    public async Task<IActionResult> Edit(int id, [Bind("StoreId,StoreName,StoreAddress,SelectedPaymentMethods")] Store store)
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
