using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class LocationPostController : Controller
    {
        private readonly ILocationPostService _locationPostService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocationPostController(ILocationPostService locationPostService, IWebHostEnvironment webHostEnvironment)
        {
            _locationPostService = locationPostService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Index
        public async Task<IActionResult> Index()
        {
            var posts = await _locationPostService.GetAllPostsAsync();
            return View(posts);
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new LocationPostCreateViewModel
            {
                Tags = await _locationPostService.GetTagsAsync()
            };

            return View(model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LocationPostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tags = await _locationPostService.GetTagsAsync();
                return View(model);
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _locationPostService.CreatePostAsync(model, userId, uploadsFolder);

            TempData["Success"] = "Post created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var model = await _locationPostService.GetPostForEditAsync(id, userId);
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, LocationPostEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Tags = await _locationPostService.GetTagsAsync();
                return View(model);
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                await _locationPostService.EditPostAsync(model, userId, uploadsFolder);
                TempData["Success"] = "Post edited successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: Details
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var model = await _locationPostService.GetPostDetailsAsync(id);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var model = await _locationPostService.GetPostForDeleteAsync(id, userId);
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(LocationPostDeleteViewModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _locationPostService.DeletePostAsync(model, userId);
                TempData["Success"] = "Post deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToAction(nameof(Index));
            }
        }










    }
}