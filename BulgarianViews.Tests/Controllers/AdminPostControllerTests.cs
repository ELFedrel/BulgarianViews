using BulgarianViews.Areas.Admin.Controllers;
using BulgarianViews.Data.Models;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Admin;
using BulgarianViews.Web.ViewModels.LocationPost;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Controllers
{
    [TestFixture]
    public class AdminPostControllerTests
    {
        private Mock<ILocationPostService> _locationPostServiceMock;
        private AdminPostController _controller;

        [SetUp]
        public void SetUp()
        {
            _locationPostServiceMock = new Mock<ILocationPostService>();

            _controller = new AdminPostController(_locationPostServiceMock.Object)
            {
                TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                    new DefaultHttpContext(),
                    Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>())
            };
        }


        [Test]
        public async Task Index_ShouldReturnViewWithPosts()
        {
            // Arrange
            var posts = new List<LocationPostIndexViewModel>
            {
                new LocationPostIndexViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Post 1",
                    Description = "Description 1",
                    UserName = "User1",
                    AverageRating = 4.5,
                    PhotoURL = "/images/post1.png"
                },
                new LocationPostIndexViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Post 2",
                    Description = "Description 2",
                    UserName = "User2",
                    AverageRating = 4.0,
                    PhotoURL = "/images/post2.png"
                }
            };

            _locationPostServiceMock.Setup(s => s.GetAllPostsAsync()).ReturnsAsync(posts);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<ManagePostViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(posts.Count, model.Count);
            Assert.AreEqual(posts[0].Title, model[0].Title);
        }

        [Test]
        public async Task Delete_ShouldRemovePost_WhenValidIdProvided()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Title = "Sample Post",
                UserId = Guid.NewGuid()
            };

            
            _locationPostServiceMock.Setup(s => s.GetById(postId)).ReturnsAsync(post);
            _locationPostServiceMock
                .Setup(s => s.DeletePostAsync(It.IsAny<LocationPostDeleteViewModel>(), post.UserId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("Post deleted successfully!", _controller.TempData["Success"]);
        }


        [Test]
        public async Task Delete_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _locationPostServiceMock.Setup(s => s.GetById(postId)).ReturnsAsync((LocationPost)null);

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Post not found.", notFoundResult.Value);
        }

        [Test]
        public async Task Delete_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Title = "Sample Post",
                UserId = Guid.NewGuid()
            };

            _locationPostServiceMock.Setup(s => s.GetById(postId)).ReturnsAsync(post);
            _locationPostServiceMock
                .Setup(s => s.DeletePostAsync(It.IsAny<LocationPostDeleteViewModel>(), post.UserId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("An error occurred while deleting the post: Unexpected error", _controller.TempData["Error"]);
        }

        [Test]
        public async Task Delete_ShouldReturnUnauthorized_WhenAccessIsDenied()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var post = new LocationPost
            {
                Id = postId,
                Title = "Sample Post",
                UserId = Guid.NewGuid()
            };

            _locationPostServiceMock.Setup(s => s.GetById(postId)).ReturnsAsync(post);
            _locationPostServiceMock
                .Setup(s => s.DeletePostAsync(It.IsAny<LocationPostDeleteViewModel>(), post.UserId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.Delete(postId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("You don't have permission to delete this post.", _controller.TempData["Error"]);
        }



    }
}
