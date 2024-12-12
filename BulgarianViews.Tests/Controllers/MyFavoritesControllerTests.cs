using BulgarianViews.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.LocationPost;
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
    public class MyFavoritesControllerTests
    {
        private Mock<IMyFavoritesService> _myFavoritesServiceMock;
        private MyFavoritesController _controller;

        [SetUp]
        public void SetUp()
        {
            _myFavoritesServiceMock = new Mock<IMyFavoritesService>();

            _controller = new MyFavoritesController(_myFavoritesServiceMock.Object);

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
        public async Task Index_ShouldReturnViewWithFavorites()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = new List<FavoritesViewModel>
            {
                new FavoritesViewModel { Id = Guid.NewGuid(), Title = "Favorite 1" },
                new FavoritesViewModel { Id = Guid.NewGuid(), Title = "Favorite 2" }
            };

            _myFavoritesServiceMock.Setup(s => s.GetUserFavoritesAsync(userId)).ReturnsAsync(favorites);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(favorites, viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldHandleEmptyFavoritesList()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _myFavoritesServiceMock.Setup(s => s.GetUserFavoritesAsync(userId)).ReturnsAsync(new List<FavoritesViewModel>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.IsEmpty((List<FavoritesViewModel>)viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldHandleExceptionGracefully()
        {
            // Arrange
            var userId = Guid.Parse(_controller.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _myFavoritesServiceMock.Setup(s => s.GetUserFavoritesAsync(userId)).ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOf<StatusCodeResult>(result);
        }

        

        

        [Test]
        public async Task Add_ShouldRedirectToIndex_WithSuccessMessage()
        {
            // Arrange
            var locationId = Guid.NewGuid();

            _myFavoritesServiceMock.Setup(s => s.AddToFavoritesAsync(It.IsAny<Guid>(), locationId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Add(locationId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("Added to favorites!", _controller.TempData["Success"]);
        }

     

      

        

        [Test]
        public async Task Remove_ShouldRedirectToIndex_WithSuccessMessage()
        {
            // Arrange
            var locationId = Guid.NewGuid();

            _myFavoritesServiceMock.Setup(s => s.RemoveFromFavoritesAsync(It.IsAny<Guid>(), locationId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Remove(locationId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("Removed from favorites!", _controller.TempData["Success"]);
        }

        [Test]
        public async Task Remove_ShouldRedirectToIndex_WithErrorMessage_WhenFavoriteNotFound()
        {
            // Arrange
            var locationId = Guid.NewGuid();

            _myFavoritesServiceMock.Setup(s => s.RemoveFromFavoritesAsync(It.IsAny<Guid>(), locationId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Remove(locationId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);
            Assert.AreEqual("Favorite not found.", _controller.TempData["Error"]);
        }

        

        
    }
}
