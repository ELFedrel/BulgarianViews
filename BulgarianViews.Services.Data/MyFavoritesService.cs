using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class MyFavoritesService : IMyFavoritesService
    {
        private readonly IFavoriteViewsRepository _favoriteViewsRepository;

        public MyFavoritesService(IFavoriteViewsRepository favoriteViewsRepository)
        {
            _favoriteViewsRepository = favoriteViewsRepository;
        }

        public async Task<List<FavoritesViewModel>> GetUserFavoritesAsync(Guid userId)
        {
            var favorites = await _favoriteViewsRepository.GetUserFavoritesAsync(userId);

            return favorites.Select(f => new FavoritesViewModel
            {
                Id = f.LocationId,
                Title = f.Location.Title,
                Description = f.Location.Description,
                PhotoURL = f.Location.PhotoURL
            }).ToList();
        }

        public async Task AddToFavoritesAsync(Guid userId, Guid locationId)
        {
            if (locationId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(locationId), "LocationId cannot be empty.");
            }

            var favorite = new FavoriteViews
            {
                UserId = userId,
                LocationId = locationId
            };

            await _favoriteViewsRepository.AddFavoriteAsync(favorite);
        }

        public async Task RemoveFromFavoritesAsync(Guid userId, Guid locationId)
        {
            var result = await _favoriteViewsRepository.RemoveFavoriteAsync(userId, locationId);
            if (!result)
            {
                throw new KeyNotFoundException("Favorite not found.");
            }
        }
    }
}
