using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data;
using BulgarianViews.Web.ViewModels.Profile;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private Mock<IRepository<ApplicationUser, Guid>> _userRepositoryMock;
        private Mock<IRepository<LocationPost, Guid>> _postRepositoryMock;
        private ProfileService _profileService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IRepository<ApplicationUser, Guid>>();
            _postRepositoryMock = new Mock<IRepository<LocationPost, Guid>>();

            _profileService = new ProfileService(
                _userRepositoryMock.Object,
                _postRepositoryMock.Object
            );
        }

        [Test]
        public async Task GetProfileAsync_ShouldReturnProfile_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "TestUser",
                ProfilePictureURL = "/images/test.png",
                Bio = "Test bio"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _profileService.GetProfileAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("TestUser", result.UserName);
            Assert.AreEqual("/images/test.png", result.ProfilePictureURL);
            Assert.AreEqual("Test bio", result.Bio);
        }

        [Test]
        public void GetProfileAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _profileService.GetProfileAsync(userId));
        }

        [Test]
        public async Task GetProfileForEditAsync_ShouldReturnProfileForEdit_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "EditableUser",
                Bio = "Editable bio",
                ProfilePictureURL = "/images/edit.png"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _profileService.GetProfileForEditAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("EditableUser", result.UserName);
            Assert.AreEqual("Editable bio", result.Bio);
            Assert.AreEqual("/images/edit.png", result.ProfilePictureURL);
        }

        [Test]
        public void GetProfileForEditAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _profileService.GetProfileForEditAsync(userId));
        }

        [Test]
        public async Task UpdateProfileAsync_ShouldUpdateProfile_WhenValidInput()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "OldUser",
                Bio = "Old bio"
            };

            var model = new ProfileEditViewModel
            {
                UserName = "UpdatedUser",
                Bio = "Updated bio"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            await _profileService.UpdateProfileAsync(userId, model);

            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<ApplicationUser>(u =>
                u.UserName == "UpdatedUser" &&
                u.Bio == "Updated bio")), Times.Once);
        }

        [Test]
        public void UpdateProfileAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var model = new ProfileEditViewModel
            {
                UserName = "UpdatedUser",
                Bio = "Updated bio"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _profileService.UpdateProfileAsync(userId, model));
        }

        [Test]
        public async Task GetUserPostsAsync_ShouldReturnPosts_WhenPostsExist()
        {
            var userId = Guid.NewGuid();
            var posts = new List<LocationPost>
            {
                new LocationPost
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Post 1",
                    Description = "Description 1",
                    PhotoURL = "/images/post1.png",
                    AverageRating = 4.5
                },
                new LocationPost
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Title = "Post 2",
                    Description = "Description 2",
                    PhotoURL = "/images/post2.png",
                    AverageRating = 4.0
                }
            };

            _postRepositoryMock.Setup(repo => repo.FindAsync(lp => lp.UserId == userId))
                .ReturnsAsync(posts);

            var result = await _profileService.GetUserPostsAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Post 1", result.First().Title);
        }

        [Test]
        public async Task GetUserPostsAsync_ShouldReturnEmptyList_WhenNoPostsExist()
        {
            var userId = Guid.NewGuid();
            _postRepositoryMock.Setup(repo => repo.FindAsync(lp => lp.UserId == userId))
                .ReturnsAsync(new List<LocationPost>());

            var result = await _profileService.GetUserPostsAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public async Task GetUserDetailsAsync_ShouldReturnDetails_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "DetailsUser",
                ProfilePictureURL = "/images/details.png",
                Bio = "Details bio"
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            var result = await _profileService.GetUserDetailsAsync(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual("DetailsUser", result.UserName);
            Assert.AreEqual("/images/details.png", result.ProfilePictureURL);
            Assert.AreEqual("Details bio", result.Bio);
        }

        [Test]
        public void GetUserDetailsAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _profileService.GetUserDetailsAsync(userId));
        }



       

        [Test]
        public async Task UpdateProfileAsync_ShouldNotUpdateProfilePicture_WhenProfilePictureIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser
            {
                Id = userId,
                UserName = "OldUser",
                Bio = "Old bio",
                ProfilePictureURL = "/images/old.png"
            };

            var model = new ProfileEditViewModel
            {
                UserName = "UpdatedUser",
                Bio = "Updated bio",
                ProfilePicture = null 
            };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _profileService.UpdateProfileAsync(userId, model);

            // Assert
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<ApplicationUser>(u =>
                u.UserName == "UpdatedUser" &&
                u.Bio == "Updated bio" &&
                u.ProfilePictureURL == "/images/old.png")), Times.Once); 
        }

       

        [Test]
        public async Task GetUserPostsAsync_ShouldReturnPostsWithCorrectAverageRating()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var posts = new List<LocationPost>
    {
        new LocationPost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Post 1",
            AverageRating = 4.5
        },
        new LocationPost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Post 2",
            AverageRating = 3.0
        }
    };

            _postRepositoryMock.Setup(repo => repo.FindAsync(lp => lp.UserId == userId))
                .ReturnsAsync(posts);

            // Act
            var result = await _profileService.GetUserPostsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(4.5, result.First().AverageRating);
            Assert.AreEqual(3.0, result.Last().AverageRating);
        }










    }
}
