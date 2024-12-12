using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class MyFavoritesServiceTests
    {
        private Mock<IFavoriteViewsRepository> _favoriteViewsRepositoryMock;
        private MyFavoritesService _myFavoritesService;

        [SetUp]
        public void SetUp()
        {
            _favoriteViewsRepositoryMock = new Mock<IFavoriteViewsRepository>();
            _myFavoritesService = new MyFavoritesService(_favoriteViewsRepositoryMock.Object);
        }

        [Test]
        public async Task GetUserFavoritesAsync_ShouldReturnFavorites_WhenFavoritesExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var favorites = new List<FavoriteViews>
            {
                new FavoriteViews
                {
                    UserId = userId,
                    LocationId = Guid.NewGuid(),
                    Location = new LocationPost { Title = "Title 1", Description = "Description 1", PhotoURL = "/images/1.jpg" }
                },
                new FavoriteViews
                {
                    UserId = userId,
                    LocationId = Guid.NewGuid(),
                    Location = new LocationPost { Title = "Title 2", Description = "Description 2", PhotoURL = "/images/2.jpg" }
                }
            };

            _favoriteViewsRepositoryMock.Setup(repo => repo.GetUserFavoritesAsync(userId)).ReturnsAsync(favorites);

            // Act
            var result = await _myFavoritesService.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Title 1", result.First().Title);
        }

        [Test]
        public async Task GetUserFavoritesAsync_ShouldReturnEmptyList_WhenNoFavoritesExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _favoriteViewsRepositoryMock.Setup(repo => repo.GetUserFavoritesAsync(userId)).ReturnsAsync(new List<FavoriteViews>());

            // Act
            var result = await _myFavoritesService.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetUserFavoritesAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.GetUserFavoritesAsync(userId)).ReturnsAsync(new List<FavoriteViews>());

            // Act
            await _myFavoritesService.GetUserFavoritesAsync(userId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo => repo.GetUserFavoritesAsync(userId), Times.Once);
        }

        [Test]
        public async Task AddToFavoritesAsync_ShouldAddFavorite_WhenValidDataIsProvided()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            // Act
            await _myFavoritesService.AddToFavoritesAsync(userId, locationId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo =>
                repo.AddFavoriteAsync(It.Is<FavoriteViews>(fv =>
                    fv.UserId == userId && fv.LocationId == locationId)), Times.Once);
        }

        [Test]
        public async Task AddToFavoritesAsync_ShouldNotThrowException_WhenAddingFavorite()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            // Act & Assert
            Assert.DoesNotThrowAsync(() => _myFavoritesService.AddToFavoritesAsync(userId, locationId));
        }

        [Test]
        public async Task AddToFavoritesAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            // Act
            await _myFavoritesService.AddToFavoritesAsync(userId, locationId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo =>
                repo.AddFavoriteAsync(It.IsAny<FavoriteViews>()), Times.Once);
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_ShouldRemoveFavorite_WhenFavoriteExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(userId, locationId)).ReturnsAsync(true);

            // Act
            await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo => repo.RemoveFavoriteAsync(userId, locationId), Times.Once);
        }

        [Test]
        public void RemoveFromFavoritesAsync_ShouldThrowKeyNotFoundException_WhenFavoriteDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(userId, locationId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId));
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(userId, locationId)).ReturnsAsync(true);

            // Act
            await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo => repo.RemoveFavoriteAsync(userId, locationId), Times.Once);
        }









        [Test]
        public async Task GetUserFavoritesAsync_ShouldMapFavoritesToViewModelCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var favorites = new List<FavoriteViews>
    {
        new FavoriteViews
        {
            UserId = userId,
            LocationId = Guid.NewGuid(),
            Location = new LocationPost { Title = "Test Title", Description = "Test Description", PhotoURL = "/images/test.jpg" }
        }
    };

            _favoriteViewsRepositoryMock.Setup(repo => repo.GetUserFavoritesAsync(userId)).ReturnsAsync(favorites);

            // Act
            var result = await _myFavoritesService.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test Title", result[0].Title);
            Assert.AreEqual("Test Description", result[0].Description);
            Assert.AreEqual("/images/test.jpg", result[0].PhotoURL);
        }

        [Test]
        public void AddToFavoritesAsync_ShouldThrowArgumentNullException_WhenLocationIdIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            Guid locationId = Guid.Empty;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _myFavoritesService.AddToFavoritesAsync(userId, locationId));
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_ShouldNotThrowException_WhenFavoriteExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(userId, locationId)).ReturnsAsync(true);

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
                await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId));
        }

        [Test]
        public async Task GetUserFavoritesAsync_ShouldReturnCorrectNumberOfFavorites()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var favorites = new List<FavoriteViews>
    {
        new FavoriteViews
        {
            UserId = userId,
            LocationId = Guid.NewGuid(),
            Location = new LocationPost { Title = "Title 1", Description = "Description 1", PhotoURL = "/images/1.jpg" }
        },
        new FavoriteViews
        {
            UserId = userId,
            LocationId = Guid.NewGuid(),
            Location = new LocationPost { Title = "Title 2", Description = "Description 2", PhotoURL = "/images/2.jpg" }
        },
        new FavoriteViews
        {
            UserId = userId,
            LocationId = Guid.NewGuid(),
            Location = new LocationPost { Title = "Title 3", Description = "Description 3", PhotoURL = "/images/3.jpg" }
        }
    };

            _favoriteViewsRepositoryMock.Setup(repo => repo.GetUserFavoritesAsync(userId)).ReturnsAsync(favorites);

            // Act
            var result = await _myFavoritesService.GetUserFavoritesAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public async Task RemoveFromFavoritesAsync_ShouldNotModifyRepository_WhenKeyNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var locationId = Guid.NewGuid();

            _favoriteViewsRepositoryMock.Setup(repo => repo.RemoveFavoriteAsync(userId, locationId)).ReturnsAsync(false);

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _myFavoritesService.RemoveFromFavoritesAsync(userId, locationId));
            _favoriteViewsRepositoryMock.Verify(repo => repo.RemoveFavoriteAsync(userId, locationId), Times.Once);
        }

    }
}
