using BulgarianViews.Data.Models;
using BulgarianViews.Web.ViewModels.LocationPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BulgarianViews.Web.ViewModels.Home
{
    public class HomeViewModel
    {

        public List<LocationPostIndexViewModel> TopRatedPosts { get; set; } = new List<LocationPostIndexViewModel>();
        public int TotalUsers { get; set; }
        public string? UserName { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? Bio { get; set; }

    }
}
