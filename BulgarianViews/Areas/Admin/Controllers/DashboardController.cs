using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulgarianViews.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IHomeService _homeService;

        public DashboardController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _homeService.GetTotalUsersAsync();
            var totalPosts = await _homeService.GetTotalPostsAsync();
            var totalComments = await _homeService.GetTotalCommentsAsync();
            var recentActivities = await _homeService.GetRecentActivitiesAsync();

            var model = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                TotalComments = totalComments,
                RecentActivities = recentActivities
            };

            return View(model);
        }
    }
}
