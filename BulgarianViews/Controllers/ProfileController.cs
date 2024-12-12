using BulgarianViews.Data;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var model = await _profileService.GetProfileAsync(userId);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(); 
            }
            catch (Exception)
            {
                
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
           

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var model = await _profileService.GetProfileForEditAsync(userId);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _profileService.UpdateProfileAsync(userId, model);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var model = await _profileService.GetUserDetailsAsync(id);
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> MyPosts()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var posts = await _profileService.GetUserPostsAsync(userId);
            return View(posts);
        }

    }
}
