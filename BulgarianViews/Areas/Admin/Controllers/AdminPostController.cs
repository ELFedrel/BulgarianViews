using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Admin;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulgarianViews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminPostController : Controller
    {
        private readonly ILocationPostService _locationPostService;

        public AdminPostController(ILocationPostService locationPostService)
        {
            _locationPostService = locationPostService;
        }

       
        public async Task<IActionResult> Index()
        {
            var posts = await _locationPostService.GetAllPostsAsync();

            var model = posts.Select(p => new ManagePostViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                PublisherName = p.UserName,
                AverageRating = p.AverageRating,
                PhotoURL = p.PhotoURL,
                
            }).ToList();

            return View(model);
        }

       
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var post = await _locationPostService.GetById(id);

                if (post == null)
                {
                    return NotFound("Post not found.");
                }

             

                await _locationPostService.DeletePostAsync(new LocationPostDeleteViewModel{ Id = id }, post.UserId);

                TempData["Success"] = "Post deleted successfully!";
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "You don't have permission to delete this post.");
            }
            catch (KeyNotFoundException)
            {
                ModelState.AddModelError("", "The post was not found.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while deleting the post: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
