using BulgarianViews.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Repositories.Interfaces
{
    public interface IFavoriteViewsRepository 
    {
        Task<List<FavoriteViews>> GetUserFavoritesAsync(Guid userId);
        Task AddFavoriteAsync(FavoriteViews favorite);
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid locationId);
    }
}
