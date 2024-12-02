using BulgarianViews.Data.Models;
using BulgarianViews.Data;
using BulgarianViews.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BulgarianViews.Web.ViewModels.Home;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;

namespace BulgarianViews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
           
        }

        public async Task<IActionResult> Index()
        {
            var topRatedPosts = await _context.LocationPosts
         .OrderByDescending(p => p.AverageRating)
         .Take(4)
         .Select(p => new LocationPostIndexViewModel
         {
             Id = p.Id,
             Title = p.Title,
             Description = p.Description,
             PhotoURL = p.PhotoURL,
             UserName = p.User.UserName,
             AverageRating = p.AverageRating
         })
         .ToListAsync();

            
            var recentPosts = await _context.LocationPosts
                .OrderByDescending(p => p.Id)
                .Take(2)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .ToListAsync();

            
            var mostCommentedPosts = await _context.LocationPosts
                .OrderByDescending(p => p.Comments.Count)
                .Take(2)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .ToListAsync();

            
           

           
            var randomPost = await _context.LocationPosts
                .OrderBy(r => Guid.NewGuid())
                .Take(1)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .FirstOrDefaultAsync();

            
            var totalUsers = await _context.Users.CountAsync();
            var totalPosts = await _context.LocationPosts.CountAsync();
            var totalComments = await _context.Comments.CountAsync();

            var model = new HomeViewModel
            {
                TopRatedPosts = topRatedPosts,
                RecentPosts = recentPosts,
                MostCommentedPosts = mostCommentedPosts,
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                TotalComments = totalComments,
                
                RandomPost = randomPost
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
