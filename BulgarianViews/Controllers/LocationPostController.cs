using BulgarianViews.Data;
using BulgarianViews.Data.Models;
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

            string photoUrl = null;
            if (model.PhotoURL != null && model.PhotoURL.Length > 0)
            {
                // Define the folder for uploads
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{model.PhotoURL.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PhotoURL.CopyToAsync(fileStream);
                }

                // Set the photo URL to be saved in the database
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
                TagId = model.TagId
                

            };

            _context.LocationPosts.Add(newPost);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var post = await _context.LocationPosts
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var model = new LocationPostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ExistingPhotoURL = post.PhotoURL,
                TagId = post.TagId,
                Tags = await _context.Tags.ToListAsync()
                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, LocationPostEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload the tags if the model state is invalid
                model.Tags = await _context.Tags.ToListAsync();
                return View(model);
            }

            var post = await _context.LocationPosts
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            
            post.Title = model.Title;
            post.Description = model.Description;
            post.TagId = model.TagId;

            
            if (model.NewPhoto != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{model.NewPhoto.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewPhoto.CopyToAsync(fileStream);
                }

                
                if (!string.IsNullOrEmpty(post.PhotoURL))
                {
                    var oldPhotoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", post.PhotoURL.TrimStart('/'));
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        System.IO.File.Delete(oldPhotoPath);
                    }
                }

                post.PhotoURL = $"/images/{uniqueFileName}";
            }

            
           
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var post = await _context.LocationPosts
                .Include(lp => lp.User)
                .Include(lp => lp.Tag)
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var model = new LocationPostDetailsViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                PhotoURL = post.PhotoURL,
                UserName = post.User.UserName ?? String.Empty,
                TagName = post.Tag.Name
                
            };

            return View(model);
        }






    }
}
