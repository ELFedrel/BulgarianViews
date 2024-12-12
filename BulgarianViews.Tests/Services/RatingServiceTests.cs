using BulgarianViews.Data.Models;
using BulgarianViews.Services.Data;
using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BulgarianViews.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class RatingServiceTests
    {
        private Mock<IRepository<Rating, Guid>> _ratingRepositoryMock;
        private Mock<IRepository<LocationPost, Guid>> _postRepositoryMock;
        private RatingService _ratingService;

        [SetUp]
        public void SetUp()
        {
            _ratingRepositoryMock = new Mock<IRepository<Rating, Guid>>();
            _postRepositoryMock = new Mock<IRepository<LocationPost, Guid>>();

            _ratingService = new RatingService(
                _ratingRepositoryMock.Object,
                _postRepositoryMock.Object
            );
        }

        [Test]
        public async Task RateAsync_ShouldAddNewRating_WhenRatingDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Ratings = new List<Rating>()
            };

            _postRepositoryMock
                .Setup(repo => repo.GetByIdIncludingAsync(
                    postId,
                    It.IsAny<Expression<Func<LocationPost, object>>[]>()))
                .ReturnsAsync(post);

            _ratingRepositoryMock
                .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Rating, bool>>>()))
                .ReturnsAsync(new List<Rating>());

            // Act
            var result = await _ratingService.RateAsync(postId, userId, 5);

            // Assert
            Assert.IsTrue(result);

            // Проверка на извикването на AddAsync
            _ratingRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Rating>(r => r.Value == 5)), Times.Once);

            // Проверка на стойността на AverageRating
            Assert.AreEqual(5, post.AverageRating);

            // Проверка на извикването на UpdateAsync
            _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<LocationPost>(p => p.AverageRating == 5)), Times.Once);
        }



        [Test]
        public async Task RateAsync_ShouldUpdateRating_WhenRatingExists()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingRating = new Rating
            {
                Id = Guid.NewGuid(),
                LocationPostId = postId,
                UserId = userId,
                Value = 3
            };

            var post = new LocationPost
            {
                Id = postId,
                Ratings = new List<Rating> { existingRating }
            };

            _postRepositoryMock
                .Setup(repo => repo.GetByIdIncludingAsync(
                    postId,
                    It.IsAny<Expression<Func<LocationPost, object>>[]>()))
                .ReturnsAsync(post);

            _ratingRepositoryMock
                .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Rating, bool>>>()))
                .ReturnsAsync(new List<Rating> { existingRating });

            // Act
            var result = await _ratingService.RateAsync(postId, userId, 5);

            // Assert
            Assert.IsTrue(result);
            _ratingRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Rating>(r => r.Value == 5)), Times.Once);
            _postRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<LocationPost>(p => p.AverageRating == 5)), Times.Once);
        }

        [Test]
        public void RateAsync_ShouldThrowException_WhenRatingIsOutOfRange()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _ratingService.RateAsync(postId, userId, 6));
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _ratingService.RateAsync(postId, userId, 0));
        }


        [Test]
        public async Task RateAsync_ShouldUpdateAverageRating_WhenNewRatingIsAdded()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var rating = new Rating
            {
                Id = Guid.NewGuid(),
                LocationPostId = postId,
                UserId = userId1,
                Value = 4
            };

            var post = new LocationPost
            {
                Id = postId,
                Ratings = new List<Rating> { rating }
            };

            _postRepositoryMock
                .Setup(repo => repo.GetByIdIncludingAsync(
                    postId,
                    It.IsAny<Expression<Func<LocationPost, object>>[]>()))
                .ReturnsAsync(post);

            _ratingRepositoryMock
                .Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Rating, bool>>>()))
                .ReturnsAsync(new List<Rating> { rating });

            // Act
            var result = await _ratingService.RateAsync(postId, userId2, 5);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(5, post.AverageRating); 
        }

    }
}
