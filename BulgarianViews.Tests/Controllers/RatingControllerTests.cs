using BulgarianViews.Controllers;
using BulgarianViews.Services.Data.Interfaces;
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
    public class RatingControllerTests
    {
        private Mock<IRatingService> _ratingServiceMock;
        private RatingController _controller;

        [SetUp]
        public void SetUp()
        {
            _ratingServiceMock = new Mock<IRatingService>();

            _controller = new RatingController(_ratingServiceMock.Object);

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
        public async Task Rate_ShouldRedirectToDetails_WithSuccessMessage_WhenValidRating()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var rating = 5;

            _ratingServiceMock.Setup(s => s.RateAsync(postId, It.IsAny<Guid>(), rating))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Rate(postId, rating);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual(postId, redirectResult.RouteValues["id"]);
            Assert.AreEqual("Thank you for rating!", _controller.TempData["Success"]);
        }

        [Test]
        public async Task Rate_ShouldRedirectToDetails_WithErrorMessage_WhenRatingIsInvalid()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var invalidRating = 6;

            // Act
            var result = await _controller.Rate(postId, invalidRating);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual(postId, redirectResult.RouteValues["id"]);
            Assert.AreEqual("Invalid rating. Please select a value between 1 and 5.", _controller.TempData["Error"]);
        }

        [Test]
        public async Task Rate_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var rating = 4;

            _ratingServiceMock.Setup(s => s.RateAsync(postId, It.IsAny<Guid>(), rating))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Rate(postId, rating);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual("LocationPost", redirectResult.ControllerName);
            Assert.AreEqual(postId, redirectResult.RouteValues["id"]);
            Assert.AreEqual("An error occurred: Unexpected error", _controller.TempData["Error"]);
        }

        [Test]
        public async Task Rate_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No user

            var postId = Guid.NewGuid();
            var rating = 3;

            // Act
            var result = await _controller.Rate(postId, rating);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Rate_ShouldCallServiceMethod_WhenValidRating()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var rating = 4;
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));

            _ratingServiceMock.Setup(s => s.RateAsync(postId, userId, rating)).ReturnsAsync(true);

            // Act
            await _controller.Rate(postId, rating);

            // Assert
            _ratingServiceMock.Verify(s => s.RateAsync(postId, userId, rating), Times.Once);
        }

        [Test]
        public async Task Rate_ShouldNotCallServiceMethod_WhenRatingIsInvalid()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var invalidRating = 0;

            // Act
            await _controller.Rate(postId, invalidRating);

            // Assert
            _ratingServiceMock.Verify(s => s.RateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }

     


    }
}
