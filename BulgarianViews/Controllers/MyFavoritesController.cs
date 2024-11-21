using BulgarianViews.Data.Models;
using BulgarianViews.Data;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class MyFavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyFavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var favorites = await _context.FavoriteViews
                .Where(f => f.UserId == Guid.Parse(userId))
                .Include(f => f.Location)
                .Select(f => new FavoritesViewModel
                {
                    Id = f.Location.Id,
                    Title = f.Location.Title,
                    Description = f.Location.Description,
                    PhotoURL = f.Location.PhotoURL

                })
                .ToListAsync();

            return View(favorites);
        }
        // POST: Add to Favorites
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid locationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var existingFavorite = await _context.FavoriteViews
                .FirstOrDefaultAsync(f => f.UserId == Guid.Parse(userId) && f.LocationId == locationId);

            if (existingFavorite != null)
            {
                TempData["Error"] = "This post is already in your favorites!";
                return RedirectToAction("Index", "MyFavorites");

            }

            var favorite = new FavoriteViews
            {
                UserId = Guid.Parse(userId),
                LocationId = locationId
            };

            _context.FavoriteViews.Add(favorite);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Added to favorites!";
            return RedirectToAction("Index", "MyFavorites");
        }

        // POST: Remove from Favorites
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid locationId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var favorite = await _context.FavoriteViews
                .FirstOrDefaultAsync(f => f.UserId == Guid.Parse(userId) && f.LocationId == locationId);

            if (favorite == null)
            {
                TempData["Error"] = "This post is not in your favorites!";
                return RedirectToAction(nameof(Index)); // Пренасочване към списъка с любими
            }

            _context.FavoriteViews.Remove(favorite);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Removed from favorites!"; // Съобщение за успех
            return RedirectToAction(nameof(Index)); // Пренасочване към списъка с любими
        }
    }
}
