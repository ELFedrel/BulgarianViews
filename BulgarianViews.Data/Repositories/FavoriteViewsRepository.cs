using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Repositories
{
    public class FavoriteViewsRepository : IFavoriteViewsRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteViewsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FavoriteViews>> GetUserFavoritesAsync(Guid userId)
        {
            return await _context.FavoriteViews
                .Where(f => f.UserId == userId)
                .Include(f => f.Location)
                .ToListAsync();
        }

        public async Task AddFavoriteAsync(FavoriteViews favorite)
        {
            await _context.FavoriteViews.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid locationId)
        {
            var favorite = await _context.FavoriteViews
                .FirstOrDefaultAsync(f => f.UserId == userId && f.LocationId == locationId);

            if (favorite == null)
            {
                return false;
            }

            _context.FavoriteViews.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
}
