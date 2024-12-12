using BulgarianViews.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
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
    public class CommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private CommentController _controller;

        [SetUp]
        public void SetUp()
        {
            _commentServiceMock = new Mock<ICommentService>();

            _controller = new CommentController(_commentServiceMock.Object);

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
        public async Task Add_ShouldRedirectToDetails_WhenModelIsValid()
        {
            // Arrange
            var model = new CreateCommentViewModel { Content = "Test Comment", LocationPostId = Guid.NewGuid() };
            _commentServiceMock.Setup(s => s.AddCommentAsync(It.IsAny<CreateCommentViewModel>(), It.IsAny<Guid>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
        }

        [Test]
        public async Task Add_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No user claims
            var model = new CreateCommentViewModel { Content = "Test Comment", LocationPostId = Guid.NewGuid() };

            // Act
            var result = await _controller.Add(model);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Add_ShouldRedirectToDetails_WithError_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Content", "Required");
            var model = new CreateCommentViewModel { LocationPostId = Guid.NewGuid() };

            // Act
            var result = await _controller.Add(model);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual("Failed to add comment. Please try again.", _controller.TempData["Error"]);
        }

        

       

        [Test]
        public async Task ViewComments_ShouldReturnPartialView_WithComments()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var comments = new List<CommentViewModel>
            {
                new CommentViewModel { Content = "Comment 1" },
                new CommentViewModel { Content = "Comment 2" }
            };

            _commentServiceMock.Setup(s => s.GetCommentsByPostIdAsync(postId)).ReturnsAsync(comments);

            // Act
            var result = await _controller.ViewComments(postId);

            // Assert
            var partialViewResult = result as PartialViewResult;
            Assert.IsNotNull(partialViewResult);
            Assert.AreEqual("Comments", partialViewResult.ViewName);
            Assert.AreEqual(comments, partialViewResult.Model);
        }

        [Test]
        public async Task ViewComments_ShouldReturnEmptyPartialView_WhenNoCommentsExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.GetCommentsByPostIdAsync(postId)).ReturnsAsync(new List<CommentViewModel>());

            // Act
            var result = await _controller.ViewComments(postId);

            // Assert
            var partialViewResult = result as PartialViewResult;
            Assert.IsNotNull(partialViewResult);
            Assert.AreEqual("Comments", partialViewResult.ViewName);
            Assert.IsEmpty((List<CommentViewModel>)partialViewResult.Model);
        }

        [Test]
        public async Task ViewComments_ShouldHandleServiceException()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.GetCommentsByPostIdAsync(postId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.ViewComments(postId));
        }

        

        

        [Test]
        public async Task Delete_ShouldRedirectToDetails_WithSuccess_WhenSuccessful()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _commentServiceMock.Setup(s => s.DeleteCommentAsync(commentId, userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(commentId, postId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual("Comment deleted successfully!", _controller.TempData["Success"]);
        }

        [Test]
        public async Task Delete_ShouldRedirectToDetails_WithError_WhenCommentNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _commentServiceMock.Setup(s => s.DeleteCommentAsync(commentId, userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId, postId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual("Comment not found or you do not have permission to delete it.", _controller.TempData["Error"]);
        }

        [Test]
        public async Task Delete_ShouldReturnUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();

            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No user claims

            // Act
            var result = await _controller.Delete(commentId, postId);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }


        [Test]
        public async Task Add_ShouldHandleException_WhenServiceFails()
        {
            // Arrange
            var model = new CreateCommentViewModel { Content = "Test Comment", LocationPostId = Guid.NewGuid() };
            _commentServiceMock.Setup(s => s.AddCommentAsync(It.IsAny<CreateCommentViewModel>(), It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.Add(model));
        }
       
        [Test]
        public async Task ViewComments_ShouldReturnEmptyView_WhenNoCommentsServiceFails()
        {
            // Arrange
            var postId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.GetCommentsByPostIdAsync(postId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.ViewComments(postId));
        }

        [Test]
        public async Task Delete_ShouldHandleException_WhenServiceFails()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _commentServiceMock.Setup(s => s.DeleteCommentAsync(commentId, userId))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await _controller.Delete(commentId, postId));
        }

        [Test]
        public async Task Delete_ShouldReturnForbid_WhenUserIsUnauthorized()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No user claims

            // Act
            var result = await _controller.Delete(commentId, postId);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        

        

        [Test]
        public async Task Delete_ShouldSetTempDataError_WhenCommentNotDeleted()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var postId = Guid.NewGuid();
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _commentServiceMock.Setup(s => s.DeleteCommentAsync(commentId, userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId, postId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual("Comment not found or you do not have permission to delete it.", _controller.TempData["Error"]);
        }









    }
}
