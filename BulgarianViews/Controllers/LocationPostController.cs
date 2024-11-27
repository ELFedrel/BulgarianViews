using BulgarianViews.Data;
using BulgarianViews.Data.Models;
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
                   UserName = lp.User.UserName ?? String.Empty,
                   PublisherId = lp.UserId.ToString(),
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
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) ,
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.UserId.ToString() != userId)
            {
                return Forbid(); 
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

            var comments = await _context.Comments
                .Where(c => c.LocationPostId == id)
                .Include(c => c.User)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    UserName = c.User.UserName ?? String.Empty,
                    UserId = c.UserId, 
                    DateCreated = c.DateCreated
                })
                .ToListAsync();

            var ratings = await _context.Ratings.Where(r => r.LocationPostId == id).ToListAsync();
            double averageRating = ratings.Any() ? ratings.Average(r => r.Value) : 0;

            var model = new LocationPostDetailsViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                PhotoURL = post.PhotoURL,
                UserName = post.User.UserName ?? string.Empty,
                TagName = post.Tag.Name,
                Comments = comments,
                PublisherId = post.UserId.ToString(),
                AverageRating = averageRating

            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var post = await _context.LocationPosts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.UserId.ToString() != userId)
            {
                return Forbid();
            }

            var model = new LocationPostDeleteViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(LocationPostDeleteViewModel model)
        {
            var post = await _context.LocationPosts
                .Include(p => p.Comments)  
                .Include(p => p.Ratings)  
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (post == null)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.UserId.ToString() != userId)
            {
                return Forbid();
            }

          
            _context.Comments.RemoveRange(post.Comments);

            
            _context.Ratings.RemoveRange(post.Ratings);

            
            _context.LocationPosts.Remove(post);

           
            await _context.SaveChangesAsync();

            TempData["Success"] = "Post and its related comments and ratings were deleted successfully!";
            return RedirectToAction(nameof(Index));
        }










    }
}
