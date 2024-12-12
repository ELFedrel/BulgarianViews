using BulgarianViews.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class LocationPostControllerTests
    {
        private Mock<ILocationPostService> _locationPostServiceMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private LocationPostController _controller;

        [SetUp]
        public void SetUp()
        {
            _locationPostServiceMock = new Mock<ILocationPostService>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

            _controller = new LocationPostController(
                _locationPostServiceMock.Object,
                _webHostEnvironmentMock.Object
            );

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
        public async Task Index_ShouldReturnViewWithPosts()
        {
            // Arrange
            var posts = new List<LocationPostIndexViewModel>
            {
                new LocationPostIndexViewModel { Id = Guid.NewGuid(), Title = "Post 1" },
                new LocationPostIndexViewModel { Id = Guid.NewGuid(), Title = "Post 2" }
            };
            _locationPostServiceMock.Setup(s => s.GetAllPostsAsync()).ReturnsAsync(posts);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(posts, viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldHandleEmptyPostsList()
        {
            // Arrange
            _locationPostServiceMock.Setup(s => s.GetAllPostsAsync()).ReturnsAsync(new List<LocationPostIndexViewModel>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsEmpty((List<LocationPostIndexViewModel>)viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldReturnView()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

       


        [Test]
        public async Task Create_Post_ShouldReturnViewWithModel_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Required");
            var model = new LocationPostCreateViewModel();

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

       
        [Test]
        public async Task Edit_Get_ShouldReturnView_WhenAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new LocationPostEditViewModel { Id = postId, Title = "Edit Post" };

            _locationPostServiceMock.Setup(s => s.GetPostForEditAsync(postId, userId)).ReturnsAsync(model);

            // Act
            var result = await _controller.Edit(postId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnForbid_WhenUnauthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostForEditAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.Edit(postId);

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostForEditAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Edit(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }


        

        [Test]
        public async Task Details_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostDetailsAsync(postId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Details(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Details_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostDetailsAsync(postId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Details(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result); 
        }



        [Test]
        public async Task Delete_Get_ShouldReturnView_WhenAuthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new LocationPostDeleteViewModel { Id = postId, Title = "Delete Post" };

            _locationPostServiceMock.Setup(s => s.GetPostForDeleteAsync(postId, userId)).ReturnsAsync(model);

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Delete_Get_ShouldReturnForbid_WhenUnauthorized()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostForDeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task Delete_Get_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetPostForDeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Delete_Post_ShouldRedirectToIndex_WhenSuccessful()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new LocationPostDeleteViewModel { Id = Guid.NewGuid(), Title = "Deleted Post" };

            _locationPostServiceMock.Setup(s => s.DeletePostAsync(model, userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Test]
        public async Task Delete_Post_ShouldReturnForbid_WhenUnauthorized()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new LocationPostDeleteViewModel { Id = Guid.NewGuid(), Title = "Deleted Post" };

            _locationPostServiceMock.Setup(s => s.DeletePostAsync(model, userId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.Delete(model);

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task Delete_Post_ShouldHandleMissingPost()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var model = new LocationPostDeleteViewModel { Id = Guid.NewGuid(), Title = "Missing Post" };

            _locationPostServiceMock.Setup(s => s.DeletePostAsync(model, userId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Delete(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("Post not found.", _controller.TempData["Error"]);
        }




        [Test]
        public async Task Edit_Post_ShouldReturnToView_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new LocationPostEditViewModel { Title = "Edited Post" };
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.Edit(Guid.NewGuid(), model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        








    }
}
