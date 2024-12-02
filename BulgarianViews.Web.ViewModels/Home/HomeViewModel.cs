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
        public List<LocationPostIndexViewModel> RecentPosts { get; set; } = new List<LocationPostIndexViewModel>();
        public List<LocationPostIndexViewModel> MostCommentedPosts { get; set; } = new List<LocationPostIndexViewModel>();
        public List<MostActiveUserViewModel> MostActiveUsers { get; set; } = new List<MostActiveUserViewModel>();
        public LocationPostIndexViewModel? RandomPost { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }

    }
    public class MostActiveUserViewModel
    {
        public string UserName { get; set; } = null!;
        public string ProfilePictureURL { get; set; } = null!;
        public int PostCount { get; set; }
    }
}
