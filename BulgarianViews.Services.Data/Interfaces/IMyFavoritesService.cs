using BulgarianViews.Web.ViewModels.LocationPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface IMyFavoritesService
    {
        Task<List<FavoritesViewModel>> GetUserFavoritesAsync(Guid userId);
        Task AddToFavoritesAsync(Guid userId, Guid locationId);
        Task RemoveFromFavoritesAsync(Guid userId, Guid locationId);
    }
}
