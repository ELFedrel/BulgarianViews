using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
        public int TotalComments { get; set; }
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
    }
}
