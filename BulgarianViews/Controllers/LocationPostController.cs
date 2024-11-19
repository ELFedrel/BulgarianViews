using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulgarianViews.Controllers
{
    [Authorize]
    public class LocationPostController : Controller
    {
        


        public IActionResult Index()
        {
            return View();
        }
    }
}
