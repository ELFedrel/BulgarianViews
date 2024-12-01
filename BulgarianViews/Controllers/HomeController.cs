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
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var topRatedPosts = await _context.LocationPosts
              .OrderByDescending(p => p.AverageRating)
              .Take(3)
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

            var totalUsers = await _context.Users.CountAsync();

            var model = new HomeViewModel
            {
                TopRatedPosts = topRatedPosts,
                TotalUsers = totalUsers
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
