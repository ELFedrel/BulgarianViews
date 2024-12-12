using BulgarianViews.Areas.Admin.Controllers;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Admin;
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
    public class DashboardControllerTests
    {
        private Mock<IHomeService> _homeServiceMock;
        private DashboardController _controller;

        [SetUp]
        public void SetUp()
        {
            _homeServiceMock = new Mock<IHomeService>();
            _controller = new DashboardController(_homeServiceMock.Object);
        }

        [Test]
        public async Task Index_ShouldReturnViewWithCorrectDashboardData()
        {
            var totalUsers = 5;
            var totalPosts = 10;
            var totalComments = 20;
            var recentActivities = new List<RecentActivityViewModel>
            {
                new RecentActivityViewModel { UserName = "User1", ActionDescription = "Created a post" },
                new RecentActivityViewModel { UserName = "User2", ActionDescription = "Commented on a post" }
            };

            _homeServiceMock.Setup(s => s.GetTotalUsersAsync()).ReturnsAsync(totalUsers);
            _homeServiceMock.Setup(s => s.GetTotalPostsAsync()).ReturnsAsync(totalPosts);
            _homeServiceMock.Setup(s => s.GetTotalCommentsAsync()).ReturnsAsync(totalComments);
            _homeServiceMock.Setup(s => s.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as AdminDashboardViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(totalUsers, model.TotalUsers);
            Assert.AreEqual(totalPosts, model.TotalPosts);
            Assert.AreEqual(totalComments, model.TotalComments);
            Assert.AreEqual(2, model.RecentActivities.Count);
            Assert.AreEqual("User1", model.RecentActivities[0].UserName);
            Assert.AreEqual("Created a post", model.RecentActivities[0].ActionDescription);
            Assert.AreEqual("User2", model.RecentActivities[1].UserName);
            Assert.AreEqual("Commented on a post", model.RecentActivities[1].ActionDescription);
        }

      

        [Test]
        public async Task Index_ShouldHandleEmptyRecentActivities()
        {
            var totalUsers = 5;
            var totalPosts = 10;
            var totalComments = 20;
            var recentActivities = new List<RecentActivityViewModel>();

            _homeServiceMock.Setup(s => s.GetTotalUsersAsync()).ReturnsAsync(totalUsers);
            _homeServiceMock.Setup(s => s.GetTotalPostsAsync()).ReturnsAsync(totalPosts);
            _homeServiceMock.Setup(s => s.GetTotalCommentsAsync()).ReturnsAsync(totalComments);
            _homeServiceMock.Setup(s => s.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as AdminDashboardViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.RecentActivities.Count);
        }

        [Test]
        public async Task Index_ShouldReturnCorrectCounts()
        {
            var totalUsers = 15;
            var totalPosts = 25;
            var totalComments = 35;
            var recentActivities = new List<RecentActivityViewModel>
            {
                new RecentActivityViewModel { UserName = "User1", ActionDescription = "Created a post" }
            };

            _homeServiceMock.Setup(s => s.GetTotalUsersAsync()).ReturnsAsync(totalUsers);
            _homeServiceMock.Setup(s => s.GetTotalPostsAsync()).ReturnsAsync(totalPosts);
            _homeServiceMock.Setup(s => s.GetTotalCommentsAsync()).ReturnsAsync(totalComments);
            _homeServiceMock.Setup(s => s.GetRecentActivitiesAsync()).ReturnsAsync(recentActivities);

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as AdminDashboardViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(totalUsers, model.TotalUsers);
            Assert.AreEqual(totalPosts, model.TotalPosts);
            Assert.AreEqual(totalComments, model.TotalComments);
        }
    }
}
