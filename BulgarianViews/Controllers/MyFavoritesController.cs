using BulgarianViews.Data.Models;
using BulgarianViews.Data;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BulgarianViews.Services.Data.Interfaces;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class MyFavoritesController : Controller
    {
        private readonly IMyFavoritesService _myFavoritesService;

        public MyFavoritesController(IMyFavoritesService myFavoritesService)
        {
            _myFavoritesService = myFavoritesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = await _myFavoritesService.GetUserFavoritesAsync(userId);

            return View(favorites);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Guid locationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _myFavoritesService.AddToFavoritesAsync(userId, locationId);

            TempData["Success"] = "Added to favorites!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid locationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId);
                TempData["Success"] = "Removed from favorites!";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Favorite not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
