using BulgarianViews.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Controllers
{

    [TestFixture]
    public class ProfileControllerTests
    {
        private Mock<IProfileService> _profileServiceMock;
        private ProfileController _controller;

        [SetUp]
        public void SetUp()
        {
            _profileServiceMock = new Mock<IProfileService>();
            _controller = new ProfileController(_profileServiceMock.Object);

            var userId = Guid.NewGuid().ToString();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                _controller.ControllerContext.HttpContext,
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
            );
        }

        [Test]
        public async Task Index_ShouldReturnViewWithProfile_WhenUserExists()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = new ProfileViewModel { UserName = "TestUser", Bio = "TestBio" };

            _profileServiceMock.Setup(s => s.GetProfileAsync(userId)).ReturnsAsync(profile);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(profile, viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldReturnNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _profileServiceMock.Setup(s => s.GetProfileAsync(userId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }


        [Test]
        public async Task Edit_Get_ShouldReturnViewWithProfileForEdit()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profileEdit = new ProfileEditViewModel { UserName = "EditableUser" };

            _profileServiceMock.Setup(s => s.GetProfileForEditAsync(userId)).ReturnsAsync(profileEdit);

            // Act
            var result = await _controller.Edit();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(profileEdit, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _profileServiceMock.Setup(s => s.GetProfileForEditAsync(userId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Edit();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_ShouldUpdateProfile_WhenModelIsValid()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new ProfileEditViewModel { UserName = "UpdatedUser", Bio = "UpdatedBio" };

            // Act
            var result = await _controller.Edit(model);

            // Assert
            _profileServiceMock.Verify(s => s.UpdateProfileAsync(userId, model), Times.Once);
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Test]
        public async Task Edit_Post_ShouldReturnView_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserName", "Required");
            var model = new ProfileEditViewModel();

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Details_ShouldReturnViewWithProfileDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profileDetails = new ProfileViewModel { UserName = "DetailsUser" };

            _profileServiceMock.Setup(s => s.GetUserDetailsAsync(userId)).ReturnsAsync(profileDetails);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(profileDetails, viewResult.Model);
        }

        [Test]
        public async Task Details_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _profileServiceMock.Setup(s => s.GetUserDetailsAsync(userId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Details(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task MyPosts_ShouldReturnViewWithUserPosts()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var posts = new List<MyPostViewModel>
            {
                new MyPostViewModel { Id = Guid.NewGuid(), Title = "Post 1" },
                new MyPostViewModel { Id = Guid.NewGuid(), Title = "Post 2" }
            };

            _profileServiceMock.Setup(s => s.GetUserPostsAsync(userId)).ReturnsAsync(posts);

            // Act
            var result = await _controller.MyPosts();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(posts, viewResult.Model);
        }

        [Test]
        public async Task MyPosts_ShouldHandleEmptyPosts()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _profileServiceMock.Setup(s => s.GetUserPostsAsync(userId)).ReturnsAsync(new List<MyPostViewModel>());

            // Act
            var result = await _controller.MyPosts();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsEmpty((List<MyPostViewModel>)viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _profileServiceMock.Setup(s => s.GetProfileAsync(userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Index();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", statusCodeResult.Value);
        }


    }
}
