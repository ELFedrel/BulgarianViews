using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Services.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    public class RatingController : Controller
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rate(Guid postId, int rating)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["Error"] = "Invalid rating. Please select a value between 1 and 5.";
                return RedirectToAction("Details", "LocationPost", new { id = postId });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = Guid.Parse(userIdClaim);

            try
            {
                await _ratingService.RateAsync(postId, userId, rating);
                TempData["Success"] = "Thank you for rating!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Details", "LocationPost", new { id = postId });
        }
    }
}
