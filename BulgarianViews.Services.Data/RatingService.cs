using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class RatingService : IRatingService
    {
        private readonly IRepository<Rating, Guid> _ratingRepository;
        private readonly IRepository<LocationPost, Guid> _postRepository;

        public RatingService(
            IRepository<Rating, Guid> ratingRepository,
            IRepository<LocationPost, Guid> postRepository)
        {
            _ratingRepository = ratingRepository;
            _postRepository = postRepository;
        }

        public async Task<bool> RateAsync(Guid postId, Guid userId, int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // Проверка за съществуващ рейтинг
            var existingRating = (await _ratingRepository.FindAsync(r => r.UserId == userId && r.LocationPostId == postId))
                .FirstOrDefault();

            if (existingRating != null)
            {
                existingRating.Value = rating;
                await _ratingRepository.UpdateAsync(existingRating);
            }
            else
            {
                var newRating = new Rating
                {
                    Id = Guid.NewGuid(),
                    Value = rating,
                    UserId = userId,
                    LocationPostId = postId
                };

                await _ratingRepository.AddAsync(newRating);

                var postForAddingRating = await _postRepository.GetByIdIncludingAsync(postId, p => p.Ratings);
                if (postForAddingRating != null)
                {
                    postForAddingRating.Ratings.Add(newRating);
                }
            }

            
            var post = await _postRepository.GetByIdIncludingAsync(postId, p => p.Ratings);

            if (post != null)
            {
                post.AverageRating = post.Ratings.Any() ? post.Ratings.Average(r => r.Value) : 0;
                await _postRepository.UpdateAsync(post);
            }

            return true;
        }
    }
}
