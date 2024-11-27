using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

           
            var existingRating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == Guid.Parse(userId) && r.LocationPostId == postId);

            if (existingRating != null)
            {
                existingRating.Value = rating;
            }
            else
            {
                var newRating = new Rating
                {
                    Id = Guid.NewGuid(),
                    Value = rating,
                    UserId = Guid.Parse(userId),
                    LocationPostId = postId
                };

                _context.Ratings.Add(newRating);
            }

            // Обновяване на средния рейтинг на поста
            var post = await _context.LocationPosts
                .Include(lp => lp.Ratings)
                .FirstOrDefaultAsync(lp => lp.Id == postId);

            if (post != null)
            {
                post.AverageRating = post.Ratings.Any() ? post.Ratings.Average(r => r.Value) : 0;
                _context.LocationPosts.Update(post);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Thank you for rating!";
            return RedirectToAction("Details", "LocationPost", new { id = postId });
        }
    }
}
