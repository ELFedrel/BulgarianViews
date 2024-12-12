using BulgarianViews.Areas.Admin.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
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
    public class AdminCommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private AdminCommentController _controller;

        [SetUp]
        public void SetUp()
        {
            _commentServiceMock = new Mock<ICommentService>();

            _controller = new AdminCommentController(_commentServiceMock.Object)
            {
                TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                    new DefaultHttpContext(),
                    Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>())
            };
        }

        [Test]
        public async Task Index_ShouldReturnViewWithComments()
        {
            // Arrange
            var comments = new List<CommentViewModel>
            {
                new CommentViewModel { Id = Guid.NewGuid(), Content = "Comment 1", UserName = "User1" },
                new CommentViewModel { Id = Guid.NewGuid(), Content = "Comment 2", UserName = "User2" }
            };

            _commentServiceMock.Setup(s => s.GetAllCommentsAsync()).ReturnsAsync(comments);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<CommentViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(comments.Count, model.Count);
            Assert.AreEqual(comments[0].Content, model[0].Content);
        }

        [Test]
        public async Task Delete_ShouldRedirectToIndex_WhenSuccessful()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.AdminDeleteCommentAsync(commentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
        }

        [Test]
        public async Task Delete_ShouldRedirectToIndex_WithErrorMessage_WhenDeletionFails()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.AdminDeleteCommentAsync(commentId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(commentId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
        }

        
    }
}
