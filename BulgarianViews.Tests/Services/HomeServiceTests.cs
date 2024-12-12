using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data;
using BulgarianViews.Web.ViewModels.Home;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class HomeServiceTests
    {
        private Mock<IRepository<LocationPost, Guid>> _postRepositoryMock;
        private Mock<IRepository<ApplicationUser, Guid>> _userRepositoryMock;
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private HomeService _homeService;

        [SetUp]
        public void Setup()
        {
            _postRepositoryMock = new Mock<IRepository<LocationPost, Guid>>();
            _userRepositoryMock = new Mock<IRepository<ApplicationUser, Guid>>();
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();

            _homeService = new HomeService(
                _postRepositoryMock.Object,
                _userRepositoryMock.Object,
                _commentRepositoryMock.Object
            );
        }

        

        [Test]
        public async Task GetTotalUsersAsync_ShouldReturnTotalUsers()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(10);

            // Act
            var result = await _homeService.GetTotalUsersAsync();

            // Assert
            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetTotalPostsAsync_ShouldReturnTotalPosts()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(20);

            // Act
            var result = await _homeService.GetTotalPostsAsync();

            // Assert
            Assert.AreEqual(20, result);
        }

        [Test]
        public async Task GetTotalCommentsAsync_ShouldReturnTotalComments()
        {
            // Arrange
            _commentRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(30);

            // Act
            var result = await _homeService.GetTotalCommentsAsync();

            // Assert
            Assert.AreEqual(30, result);
        }
        [Test]
        public async Task GetTotalUsersAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(5);

            // Act
            var result = await _homeService.GetTotalUsersAsync();

            // Assert
            Assert.AreEqual(5, result);
            _userRepositoryMock.Verify(repo => repo.CountAsync(), Times.Once);
        }
        [Test]
        public async Task GetTotalPostsAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(10);

            // Act
            var result = await _homeService.GetTotalPostsAsync();

            // Assert
            Assert.AreEqual(10, result);
            _postRepositoryMock.Verify(repo => repo.CountAsync(), Times.Once);
        }

        [Test]
        public async Task GetTotalCommentsAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            _commentRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(15);

            // Act
            var result = await _homeService.GetTotalCommentsAsync();

            // Assert
            Assert.AreEqual(15, result);
            _commentRepositoryMock.Verify(repo => repo.CountAsync(), Times.Once);
        }

        [Test]
        public async Task GetHomePageDataAsync_ShouldReturnEmptyData_WhenNoDataExists()
        {
            // Arrange
            _postRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(new List<LocationPost>().AsQueryable().BuildMockDbSet().Object);

            _userRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(0);
            _postRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(0);
            _commentRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(0);

            // Act
            var result = await _homeService.GetHomePageDataAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.TopRatedPosts);
            Assert.IsEmpty(result.RecentPosts);
            Assert.IsEmpty(result.MostCommentedPosts);
            Assert.AreEqual(0, result.TotalUsers);
            Assert.AreEqual(0, result.TotalPosts);
            Assert.AreEqual(0, result.TotalComments);
            Assert.IsNull(result.RandomPost);
        }






        [Test]
        public async Task GetHomePageDataAsync_ShouldReturnCorrectData_WhenDataExists()
        {
            // Arrange
            var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "TestUser" };

            var posts = new List<LocationPost>
    {
        new LocationPost
        {
            Id = Guid.NewGuid(),
            Title = "Top Rated",
            AverageRating = 5.0,
            User = user,
            Comments = new List<Comment> { new Comment() },
            PhotoURL = "top-rated.jpg"
        },
        new LocationPost
        {
            Id = Guid.NewGuid(),
            Title = "Recent Post",
            AverageRating = 4.0,
            User = user,
            Comments = new List<Comment>(),
            PhotoURL = "recent.jpg"
        }
    }.AsQueryable();

            // Mock DbSet for LocationPost
            var mockDbSet = posts.BuildMockDbSet();

            _postRepositoryMock.Setup(repo => repo.GetAllAttached()).Returns(mockDbSet.Object);

            _userRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(1);
            _postRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(2);
            _commentRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(1);

            // Act
            var result = await _homeService.GetHomePageDataAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TotalPosts);
            Assert.AreEqual(1, result.TotalUsers);
            Assert.AreEqual(1, result.TotalComments);
            Assert.AreEqual(2, result.TopRatedPosts.Count);
            Assert.AreEqual("Top Rated", result.TopRatedPosts.First().Title);
            Assert.AreEqual(2, result.RecentPosts.Count);
            Assert.IsNotNull(result.RandomPost);
        }







        [Test]
        public async Task GetTotalUsersAsync_ShouldReturnZero_WhenNoUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.CountAsync()).ReturnsAsync(0);

            // Act
            var result = await _homeService.GetTotalUsersAsync();

            // Assert
            Assert.AreEqual(0, result);
        }

       


       













    }
}
