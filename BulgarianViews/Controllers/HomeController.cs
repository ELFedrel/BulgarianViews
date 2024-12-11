using BulgarianViews.Data.Models;
using BulgarianViews.Data;
using BulgarianViews.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BulgarianViews.Web.ViewModels.Home;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;
using BulgarianViews.Services.Data.Interfaces;

namespace BulgarianViews.Controllers
{

    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _homeService.GetHomePageDataAsync();
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
