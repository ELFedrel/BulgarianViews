using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Add Comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Failed to add comment. Please try again.";
                return RedirectToAction("Details", "LocationPost", new { id = model.LocationPostId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

           var comment = new Comment
           {
               Id = Guid.NewGuid(),
               Content = model.Content,
               UserId = Guid.Parse(userId),
               LocationPostId = model.LocationPostId,
               DateCreated = DateTime.UtcNow
           };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Comment added successfully!";
            return RedirectToAction("Details", "LocationPost", new { id = model.LocationPostId });
        }

        
        [HttpGet]
        public async Task<IActionResult> ViewComments(Guid postId)
        {
            var comments = await _context.Comments
                .Where(c => c.LocationPostId == postId)
                .Include(c => c.User)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    UserName = c.User.UserName,
                    DateCreated = c.DateCreated
                })
                .ToListAsync();

            return PartialView("_Comments", comments);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid postId)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                TempData["Error"] = "Comment not found.";
                return RedirectToAction("Details", "LocationPost", new { id = postId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comment.UserId != Guid.Parse(userId))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Comment deleted successfully!";
            return RedirectToAction("Details", "LocationPost", new { id = postId });
        }
    }
}
