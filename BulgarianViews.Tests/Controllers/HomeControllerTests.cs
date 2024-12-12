using BulgarianViews.Controllers;
using BulgarianViews.Models;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Home;
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
    public class HomeControllerTests
    {
        private Mock<IHomeService> _homeServiceMock;
        private HomeController _homeController;

        [SetUp]
        public void SetUp()
        {
            _homeServiceMock = new Mock<IHomeService>();
            _homeController = new HomeController(_homeServiceMock.Object);
        }

        [Test]
        public async Task Index_ShouldReturnViewWithModel_WhenDataExists()
        {
            // Arrange
            var model = new HomeViewModel
            {
                TotalPosts = 10,
                TotalUsers = 5,
                TotalComments = 20,
                TopRatedPosts = new List<LocationPostIndexViewModel>
                {
                    new LocationPostIndexViewModel { Id = Guid.NewGuid(), Title = "Top Post" }
                }
            };

            _homeServiceMock.Setup(s => s.GetHomePageDataAsync()).ReturnsAsync(model);

            // Act
            var result = await _homeController.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public async Task Index_ShouldHandleEmptyData()
        {
            // Arrange
            var model = new HomeViewModel
            {
                TotalPosts = 0,
                TotalUsers = 0,
                TotalComments = 0,
                TopRatedPosts = new List<LocationPostIndexViewModel>()
            };

            _homeServiceMock.Setup(s => s.GetHomePageDataAsync()).ReturnsAsync(model);

            // Act
            var result = await _homeController.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
        }

        [Test]
        public void Privacy_ShouldReturnView()
        {
            // Act
            var result = _homeController.Privacy();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ShouldReturnViewWithErrorModel()
        {
            // Arrange
            var traceIdentifier = "TestTraceIdentifier";
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(h => h.TraceIdentifier).Returns(traceIdentifier);

            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            // Act
            var result = _homeController.Error();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(traceIdentifier, model.RequestId);
        }

    }
}
