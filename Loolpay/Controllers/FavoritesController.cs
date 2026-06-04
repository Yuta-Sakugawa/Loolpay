using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Loolpay.Data;
using Loolpay.Models;
using Microsoft.EntityFrameworkCore;

namespace Loolpay.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // お気に入り一覧
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var favorites = await _context.Favorites
                .Where(f => f.UserId == user.Id)
                .Include(f => f.Store)
                .ToListAsync();

            return View(favorites);
        }

        // お気に入り追加/解除 (非同期)
        [HttpPost]
        public async Task<IActionResult> Toggle(int storeId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == user.Id && f.StoreId == storeId);

            if (favorite == null)
            {
                _context.Favorites.Add(new Favorite { UserId = user.Id, StoreId = storeId });
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = true });
            }
            else
            {
                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return Json(new { success = true, isFavorite = false });
            }
        }
    }
}
