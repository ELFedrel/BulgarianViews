using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Home;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class HomeService : IHomeService
    {
        private readonly IRepository<LocationPost, Guid> _postRepository;
        private readonly IRepository<ApplicationUser, Guid> _userRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;

        public HomeService(
            IRepository<LocationPost, Guid> postRepository,
            IRepository<ApplicationUser, Guid> userRepository,
            IRepository<Comment, Guid> commentRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }

        public async Task<HomeViewModel> GetHomePageDataAsync()
        {
            var topRatedPosts = await _postRepository
                .GetAllAttached()
                .OrderByDescending(p => p.AverageRating)
                .Take(4)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .ToListAsync();

            var recentPosts = await _postRepository
                .GetAllAttached()
                .OrderByDescending(p => p.Id)
                .Take(2)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .ToListAsync();

            var mostCommentedPosts = await _postRepository
                .GetAllAttached()
                .OrderByDescending(p => p.Comments.Count)
                .Take(2)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .ToListAsync();

            var randomPost = await _postRepository
                .GetAllAttached()
                .OrderBy(r => Guid.NewGuid())
                .Take(1)
                .Select(p => new LocationPostIndexViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoURL = p.PhotoURL,
                    UserName = p.User.UserName,
                    AverageRating = p.AverageRating
                })
                .FirstOrDefaultAsync();

            var totalUsers = await _userRepository.CountAsync();
            var totalPosts = await _postRepository.CountAsync();
            var totalComments = await _commentRepository.CountAsync();

            return new HomeViewModel
            {
                TopRatedPosts = topRatedPosts,
                RecentPosts = recentPosts,
                MostCommentedPosts = mostCommentedPosts,
                TotalUsers = totalUsers,
                TotalPosts = totalPosts,
                TotalComments = totalComments,
                RandomPost = randomPost
            };
        }
    }
}
