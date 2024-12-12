using BulgarianViews.Areas.Admin.Controllers;
using BulgarianViews.Data.Models;
using BulgarianViews.Data;
using BulgarianViews.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BulgarianViews.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
        private ApplicationDbContext _dbContext;
        private UserController _controller;

        [SetUp]
        public void SetUp()
        {
            
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                Mock.Of<IRoleStore<IdentityRole<Guid>>>(), null, null, null, null);

            _controller = new UserController(_userManagerMock.Object, _roleManagerMock.Object, _dbContext);
        }

        [Test]
        public async Task Index_ShouldReturnViewWithUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), UserName = "User1", Email = "user1@example.com" },
                new ApplicationUser { Id = Guid.NewGuid(), UserName = "User2", Email = "user2@example.com" }
            };
            var roles = new List<string> { "Admin", "User" };

            _userManagerMock.Setup(m => m.Users).Returns(users.AsQueryable());
            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(roles);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as List<UserViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(users.Count, model.Count);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnView_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "TestUser", Email = "test@example.com" };
            var roles = new List<string> { "Admin", "User" };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);
            _roleManagerMock.Setup(m => m.Roles).Returns(new List<IdentityRole<Guid>> { new IdentityRole<Guid> { Name = "Admin" } }.AsQueryable());

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var model = viewResult.Model as EditUserViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(userId, model.Id);
            Assert.AreEqual(user.UserName, model.UserName);
            Assert.AreEqual(roles, model.Roles);
        }

        [Test]
        public async Task Edit_Get_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userManagerMock.Setup(m => m.FindByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Post_Should_Return_View_If_User_Not_Found()
        {
            // Arrange
            var model = new EditUserViewModel { Id = Guid.NewGuid(), UserName = "UpdatedUser", Email = "updated@example.com" };
            _userManagerMock.Setup(um => um.FindByIdAsync(model.Id.ToString())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Edit(model);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        

        [Test]
        public async Task Index_Should_Return_Empty_View_If_No_Users()
        {
            // Arrange
            _userManagerMock.Setup(um => um.Users).Returns(new List<ApplicationUser>().AsQueryable());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<UserViewModel>;
            Assert.IsEmpty(model);
        }

        [Test]
        public async Task Edit_Post_Should_Handle_Update_Failure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, UserName = "OldUser", Email = "old@example.com" };
            var model = new EditUserViewModel
            {
                Id = userId,
                UserName = "UpdatedUser",
                Email = "updated@example.com",
                Roles = new List<string> { "Admin" }
            };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed." }));

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual(model, viewResult.Model);
            Assert.AreEqual("Update failed.", _controller.ModelState[string.Empty].Errors.First().ErrorMessage);
        }

       
        










    }
}
