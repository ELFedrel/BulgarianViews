using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

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

            await _commentService.AddCommentAsync(model, Guid.Parse(userId));

            TempData["Success"] = "Comment added successfully!";
            return RedirectToAction("Details", "LocationPost", new { id = model.LocationPostId });
        }

        [HttpGet]
        public async Task<IActionResult> ViewComments(Guid postId)
        {
            var comments = await _commentService.GetCommentsByPostIdAsync(postId);
            return PartialView("Comments", comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var success = await _commentService.DeleteCommentAsync(id, Guid.Parse(userId));
            if (!success)
            {
                TempData["Error"] = "Comment not found or you do not have permission to delete it.";
            }
            else
            {
                TempData["Success"] = "Comment deleted successfully!";
            }

            return RedirectToAction("Details", "LocationPost", new { id = postId });
        }
    }
}
