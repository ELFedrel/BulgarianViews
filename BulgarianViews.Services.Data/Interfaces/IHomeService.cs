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
    }
}
