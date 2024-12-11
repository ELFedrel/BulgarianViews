using BulgarianViews.Web.ViewModels.Admin;
using BulgarianViews.Web.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomePageDataAsync();
        Task<int> GetTotalUsersAsync();
        Task<int> GetTotalPostsAsync();
        Task<int> GetTotalCommentsAsync();
        Task<List<RecentActivityViewModel>> GetRecentActivitiesAsync();
    }
}
