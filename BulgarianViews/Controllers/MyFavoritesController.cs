using Microsoft.AspNetCore.Mvc;

namespace BulgarianViews.Controllers
{
    public class MyFavoritesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
