using BulgarianViews.Data;
using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class LocationPostServiceTests
    {
        private Mock<IRepository<LocationPost, Guid>> _locationPostRepositoryMock;
        private Mock<IRepository<Tag, Guid>> _tagRepositoryMock;
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private Mock<IRepository<Rating, Guid>> _ratingRepositoryMock;
        private Mock<IFavoriteViewsRepository> _favoriteViewsRepositoryMock;
        private LocationPostService _locationPostService;

        [SetUp]
        public void Setup()
        {
            _locationPostRepositoryMock = new Mock<IRepository<LocationPost, Guid>>();
            _tagRepositoryMock = new Mock<IRepository<Tag, Guid>>();
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();
            _ratingRepositoryMock = new Mock<IRepository<Rating, Guid>>();
            _favoriteViewsRepositoryMock = new Mock<IFavoriteViewsRepository>();

            _locationPostService = new LocationPostService(
                _locationPostRepositoryMock.Object,
                _tagRepositoryMock.Object,
                _commentRepositoryMock.Object,
                _ratingRepositoryMock.Object,
                _favoriteViewsRepositoryMock.Object);
        }





        [Test]
        public async Task GetAllPostsAsync_Should_Return_List_Of_Posts()
        {
            // Arrange
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "TestUser" };

            var posts = new List<LocationPost>
    {
        new LocationPost
        {
            Id = Guid.NewGuid(),
            Title = "Test Post 1",
            Description = "Description 1",
            PhotoURL = "url1",
            UserId = user.Id,
            User = user, // Свързване на потребител
            Ratings = new List<Rating> { new Rating { Value = 5 }, new Rating { Value = 3 } }
        },
        new LocationPost
        {
            Id = Guid.NewGuid(),
            Title = "Test Post 2",
            Description = "Description 2",
            PhotoURL = "url2",
            UserId = user.Id,
            User = user, // Свързване на потребител
            Ratings = new List<Rating> { new Rating { Value = 4 } }
        }
    }.AsQueryable();

            // Мокване на IQueryable и IAsyncEnumerable
            var mockSet = new Mock<IQueryable<LocationPost>>();
            mockSet.As<IAsyncEnumerable<LocationPost>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<LocationPost>(posts.GetEnumerator()));
            mockSet.As<IQueryable<LocationPost>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<LocationPost>(posts.Provider));
            mockSet.As<IQueryable<LocationPost>>().Setup(m => m.Expression).Returns(posts.Expression);
            mockSet.As<IQueryable<LocationPost>>().Setup(m => m.ElementType).Returns(posts.ElementType);
            mockSet.As<IQueryable<LocationPost>>().Setup(m => m.GetEnumerator()).Returns(posts.GetEnumerator());

            _locationPostRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockSet.Object);

            // Act
            var result = await _locationPostService.GetAllPostsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Test Post 1", result[0].Title);
            Assert.AreEqual(4, result[1].AverageRating);
        }






        [Test]
        public async Task CreatePostAsync_ShouldAddNewPost()
        {
            // Arrange
            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var model = new LocationPostCreateViewModel
            {
                Title = "New Post",
                Description = "Description",
                PhotoURL = new Mock<IFormFile>().Object,
                TagId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();

            // Act
            await _locationPostService.CreatePostAsync(model, userId, uploadsFolder);

            // Assert
            _locationPostRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<LocationPost>()), Times.Once);

            // Cleanup
            if (Directory.Exists(uploadsFolder))
            {
                Directory.Delete(uploadsFolder, true);
            }
        }



        [Test]
        public async Task CreatePostAsync_ShouldSetCorrectProperties()
        {
            // Arrange
            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var model = new LocationPostCreateViewModel
            {
                Title = "New Post",
                Description = "Description",
                PhotoURL = new Mock<IFormFile>().Object,
                TagId = Guid.NewGuid()
            };
            var userId = Guid.NewGuid();
            LocationPost capturedPost = null;

            _locationPostRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<LocationPost>()))
                .Callback<LocationPost>(post => capturedPost = post);

            // Act
            await _locationPostService.CreatePostAsync(model, userId, uploadsFolder);

            // Assert
            Assert.IsNotNull(capturedPost);
            Assert.AreEqual("New Post", capturedPost.Title);
            Assert.AreEqual("Description", capturedPost.Description);
            Assert.AreEqual(userId, capturedPost.UserId);

            // Cleanup
            if (Directory.Exists(uploadsFolder))
            {
                Directory.Delete(uploadsFolder, true);
            }
        }





        [Test]
        public async Task DeletePostAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var differentUserId = Guid.NewGuid(); 
            var unauthorizedUserId = Guid.NewGuid(); 

            var post = new LocationPost
            {
                Id = postId,
                UserId = differentUserId 
            };

            var posts = new List<LocationPost> { post };

            _locationPostRepositoryMock
                .Setup(repo => repo.GetAllAttached())
                .Returns(posts.AsQueryable().BuildMockDbSet().Object);

            // Act & Assert
            var exception = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _locationPostService.DeletePostAsync(new LocationPostDeleteViewModel { Id = postId }, unauthorizedUserId));

            Assert.That(exception, Is.Not.Null, "Expected an UnauthorizedAccessException to be thrown.");
        }








        [Test]
        public async Task GetTagsAsync_ShouldReturnListOfTags_WhenTagsExist()
        {
            // Arrange
            var tags = new List<Tag>
        {
            new Tag { Id = Guid.NewGuid(), Name = "Nature" },
            new Tag { Id = Guid.NewGuid(), Name = "Travel" }
        };

            _tagRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(tags);

            // Act
            var result = await _locationPostService.GetTagsAsync();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Nature", result[0].Name);
            Assert.AreEqual("Travel", result[1].Name);
        }

        [Test]
        public async Task GetTagsAsync_ShouldReturnEmptyList_WhenNoTagsExist()
        {
            // Arrange
            _tagRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Tag>());

            // Act
            var result = await _locationPostService.GetTagsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public async Task GetById_ShouldReturnPost_WhenPostExists()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Title = "Test Post"
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            var result = await _locationPostService.GetById(postId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postId, result.Id);
            Assert.AreEqual("Test Post", result.Title);
        }

        [Test]
        public async Task GetById_ShouldReturnNull_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();

            _locationPostRepositoryMock.Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync((LocationPost)null);

            // Act
            var result = await _locationPostService.GetById(postId);

            // Assert
            Assert.IsNull(result);
        }















        [Test]
        public async Task GetPostForEditAsync_ShouldReturnPostForEdit_WhenUserIsAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var tag = new Tag { Id = Guid.NewGuid(), Name = "Nature" };
            var post = new LocationPost
            {
                Id = postId,
                Title = "Test Post",
                Description = "Description",
                PhotoURL = "url1",
                UserId = userId,
                Tag = tag
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(new List<LocationPost> { post }.AsQueryable().BuildMockDbSet().Object);

            _tagRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Tag> { tag });

            // Act
            var result = await _locationPostService.GetPostForEditAsync(postId, userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postId, result.Id);
            Assert.AreEqual("Test Post", result.Title);
            Assert.AreEqual("Nature", result.Tags.First().Name);
        }

        [Test]
        public void GetPostForEditAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var unauthorizedUserId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                UserId = Guid.NewGuid(), // Различен потребител
                Title = "Test Post"
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(new List<LocationPost> { post }.AsQueryable().BuildMockDbSet().Object);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _locationPostService.GetPostForEditAsync(postId, unauthorizedUserId));
        }







        [Test]
        public async Task EditPostAsync_ShouldUpdatePost_WhenUserIsAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Title = "Old Title",
                Description = "Old Description",
                PhotoURL = "oldurl",
                UserId = userId
            };

            var model = new LocationPostEditViewModel
            {
                Id = postId,
                Title = "New Title",
                Description = "New Description",
                ExistingPhotoURL = "oldurl"
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(post);

            // Act
            await _locationPostService.EditPostAsync(model, userId, "uploads");

            // Assert
            _locationPostRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<LocationPost>(p =>
                p.Title == "New Title" &&
                p.Description == "New Description")), Times.Once);
        }

        [Test]
        public void EditPostAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var unauthorizedUserId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                UserId = Guid.NewGuid() // Различен потребител
            };

            var model = new LocationPostEditViewModel { Id = postId };

            _locationPostRepositoryMock.Setup(repo => repo.GetByIdAsync(postId))
                .ReturnsAsync(post);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _locationPostService.EditPostAsync(model, unauthorizedUserId, "uploads"));
        }


        [Test]
        public async Task DeletePostAsync_ShouldDeletePostAndRelatedData_WhenUserIsAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var favorite = new FavoriteViews { UserId = userId, LocationId = postId };

            var post = new LocationPost
            {
                Id = postId,
                UserId = userId,
                Comments = new List<Comment> { new Comment { Id = Guid.NewGuid() } },
                Ratings = new List<Rating> { new Rating { Id = Guid.NewGuid() } },
                Favorites = new List<FavoriteViews> { favorite }
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(new List<LocationPost> { post }.AsQueryable().BuildMockDbSet().Object);

            _favoriteViewsRepositoryMock
                .Setup(repo => repo.RemoveFavoriteAsync(userId, postId))
                .ReturnsAsync(true); // Симулира успешно изтриване на FavoriteViews

            // Act
            await _locationPostService.DeletePostAsync(new LocationPostDeleteViewModel { Id = postId }, userId);

            // Assert
            _favoriteViewsRepositoryMock.Verify(repo => repo.RemoveFavoriteAsync(userId, postId), Times.Once);
            _commentRepositoryMock.Verify(repo => repo.RemoveRange(It.IsAny<IEnumerable<Comment>>()), Times.Once);
            _ratingRepositoryMock.Verify(repo => repo.RemoveRange(It.IsAny<IEnumerable<Rating>>()), Times.Once);
            _locationPostRepositoryMock.Verify(repo => repo.DeleteAsync(postId), Times.Once);
        }


        [Test]
        public void DeletePostAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var unauthorizedUserId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                UserId = Guid.NewGuid() // Различен потребител
            };

            _locationPostRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(new List<LocationPost> { post }.AsQueryable().BuildMockDbSet().Object);

            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _locationPostService.DeletePostAsync(new LocationPostDeleteViewModel { Id = postId }, unauthorizedUserId));
        }




    }
}
