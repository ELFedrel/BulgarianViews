using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class LocationPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationPostController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var posts = await _context.LocationPosts
               .Include(lp => lp.User)
               .Select(lp => new LocationPostIndexViewModel
               {
                   Id = lp.Id,
                   Title = lp.Title,
                   Description = lp.Description,
                   PhotoURL = lp.PhotoURL,
                   UserName = lp.User.UserName,
                   PublisherId = lp.UserId.ToString()
               })
               .ToListAsync();

            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new LocationPostCreateViewModel();
            model.Tags = await _context.Tags.ToListAsync(); 
            
            return View(model);
        }

        // Create Action (POST)
        [HttpPost]
        public async Task<IActionResult> Create(LocationPostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload the tags if the model state is invalid
                model.Tags = await _context.Tags.ToListAsync();
                return View(model);
            }

            // Save the uploaded photo to the server
            string photoUrl = null;
            if (model.PhotoURL != null)
            {
                // Define the path to save the uploaded file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = $"{Guid.NewGuid()}_{model.PhotoURL.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file to the specified path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PhotoURL.CopyToAsync(fileStream);
                }

                // Set the photoUrl to save in the database
                photoUrl = $"/images/{uniqueFileName}";
            }

            // Create a new LocationPost object
            var newPost = new LocationPost
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                PhotoURL = photoUrl,
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                

            };

            _context.LocationPosts.Add(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
