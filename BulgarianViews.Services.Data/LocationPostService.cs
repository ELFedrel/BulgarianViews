using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class LocationPostService : ILocationPostService
    {
        private readonly IRepository<LocationPost, Guid> _locationPostRepository;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly IRepository<Rating, Guid> _ratingRepository;

        public LocationPostService(
            IRepository<LocationPost, Guid> locationPostRepository,
            IRepository<Tag, Guid> tagRepository,
            IRepository<Comment, Guid> commentRepository,
            IRepository<Rating, Guid> ratingRepository)
        {
            _locationPostRepository = locationPostRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
            _ratingRepository = ratingRepository;
        }

        public async Task<List<LocationPostIndexViewModel>> GetAllPostsAsync()
        {
            var posts = await _locationPostRepository
                .GetAllAttached()
                .Include(lp => lp.User)
                .Include(lp => lp.Ratings)
                .Select(lp => new LocationPostIndexViewModel
                {
                    Id = lp.Id,
                    Title = lp.Title,
                    Description = lp.Description,
                    PhotoURL = lp.PhotoURL,
                    UserName = lp.User.UserName ?? string.Empty,
                    PublisherId = lp.UserId.ToString(),
                    AverageRating = lp.Ratings.Any() ? lp.Ratings.Average(r => r.Value) : 0
                })
                .ToListAsync();

            return posts;
        }

        public async Task<LocationPostDetailsViewModel> GetPostDetailsAsync(Guid id)
        {
            var post = await _locationPostRepository
                .GetAllAttached()
                .Include(lp => lp.User)
                .Include(lp => lp.Tag)
                .FirstOrDefaultAsync(lp => lp.Id == id);

            if (post == null)
            {
                throw new KeyNotFoundException("Post not found");
            }

            var comments = await _commentRepository
                .GetAllAttached()
                .Where(c => c.LocationPostId == id)
                .Include(c => c.User)
                .Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    UserName = c.User.UserName ?? string.Empty,
                    UserId = c.UserId,
                    DateCreated = c.DateCreated
                })
                .ToListAsync();

            var ratings = await _ratingRepository
                .GetAllAttached()
                .Where(r => r.LocationPostId == id)
                .ToListAsync();

            double averageRating = ratings.Any() ? ratings.Average(r => r.Value) : 0;

            return new LocationPostDetailsViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                PhotoURL = post.PhotoURL,
                UserName = post.User.UserName ?? string.Empty,
                TagName = post.Tag.Name,
                Comments = comments,
                PublisherId = post.UserId.ToString(),
                AverageRating = averageRating
            };
        }

        public async Task CreatePostAsync(LocationPostCreateViewModel model, Guid userId, string uploadsFolder)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{model.PhotoURL.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoURL.CopyToAsync(fileStream);
            }

            var newPost = new LocationPost
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                PhotoURL = $"/images/{uniqueFileName}",
                UserId = userId,
                TagId = model.TagId
            };

            await _locationPostRepository.AddAsync(newPost);
        }

        public async Task<LocationPostEditViewModel> GetPostForEditAsync(Guid id, Guid userId)
        {
            var post = await _locationPostRepository
                .GetAllAttached()
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null || post.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied or post not found");
            }

            return new LocationPostEditViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                ExistingPhotoURL = post.PhotoURL,
                TagId = post.TagId,
                Tags = (List<Tag>)await _tagRepository.GetAllAsync()
            };
        }

        public async Task EditPostAsync(LocationPostEditViewModel model, Guid userId, string uploadsFolder)
        {
            var post = await _locationPostRepository.GetByIdAsync(model.Id);

            if (post == null || post.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied or post not found");
            }

            post.Title = model.Title;
            post.Description = model.Description;
            post.TagId = model.TagId;

            if (model.NewPhoto != null)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{model.NewPhoto.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewPhoto.CopyToAsync(fileStream);
                }

                post.PhotoURL = $"/images/{uniqueFileName}";
            }

            await _locationPostRepository.UpdateAsync(post);
        }

        public async Task<LocationPostDeleteViewModel> GetPostForDeleteAsync(Guid id, Guid userId)
        {
            var post = await _locationPostRepository.GetByIdAsync(id);

            if (post == null || post.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied or post not found");
            }

            return new LocationPostDeleteViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description
            };
        }

        public async Task DeletePostAsync(LocationPostDeleteViewModel model, Guid userId)
        {
            var post = await _locationPostRepository
                .GetAllAttached()
                .Include(p => p.Comments)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(p => p.Id == model.Id);

            if (post == null || post.UserId != userId)
            {
                throw new UnauthorizedAccessException("Access denied or post not found");
            }

            //var favoriteViews = await _favoriteViewsRepository
            // .FindAsync(fv => fv.LocationId == model.Id);

            //_favoriteViewsRepository.RemoveRange(favoriteViews);

            //// Изтриване на самия пост
            //await _locationPostRepository.DeleteAsync(post.Id);

            _commentRepository.RemoveRange(post.Comments);


            _ratingRepository.RemoveRange(post.Ratings);


            await _locationPostRepository.DeleteAsync(post.Id);
        }


        public async Task<List<Tag>> GetTagsAsync()
        {
            
            return (await _tagRepository.GetAllAsync()).ToList();
        }
    }
}
