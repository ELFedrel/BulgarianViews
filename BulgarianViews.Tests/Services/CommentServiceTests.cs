using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data;
using BulgarianViews.Web.ViewModels.Comment;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulgarianViews.Tests.Services
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<IRepository<Comment, Guid>> _commentRepositoryMock;
        private Mock<IRepository<ApplicationUser, Guid>> _userRepositoryMock;
        private CommentService _commentService;

        [SetUp]
        public void SetUp()
        {
            _commentRepositoryMock = new Mock<IRepository<Comment, Guid>>();
            _userRepositoryMock = new Mock<IRepository<ApplicationUser, Guid>>();

            _commentService = new CommentService(
                _commentRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        
        [Test]
        public async Task AddCommentAsync_ShouldAddComment_WhenValidInput()
        {
            // Arrange
            var model = new CreateCommentViewModel
            {
                LocationPostId = Guid.NewGuid(),
                Content = "This is a test comment."
            };
            var userId = Guid.NewGuid();

            // Act
            await _commentService.AddCommentAsync(model, userId);

            // Assert
            _commentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Once);
        }


        [Test]
        public async Task AddCommentAsync_ShouldThrowException_WhenContentIsEmpty()
        {
            // Arrange
            var model = new CreateCommentViewModel
            {
                LocationPostId = Guid.NewGuid(),
                Content = "" // Празно съдържание
            };
            var userId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.AddCommentAsync(model, userId));

            Assert.AreEqual("Comment content cannot be empty.", exception.Message);
            _commentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

       


        
        [Test]
        public async Task GetCommentsByPostIdAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var comments = new List<Comment>().AsQueryable();

            _commentRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(comments);

            // Act
            var result = await _commentService.GetCommentsByPostIdAsync(postId);

            // Assert
            Assert.IsNotNull(result);
            
            Assert.AreEqual(0, result.Count());

        }

        
        [Test]
        public async Task DeleteCommentAsync_ShouldDeleteComment_WhenUserIsAuthorized()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = userId };

            
            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentService.DeleteCommentAsync(commentId, userId);

            // Assert
            Assert.IsTrue(result); 
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once); 
        }

        
        [Test]
        public async Task DeleteCommentAsync_ShouldReturnFalse_WhenUserIsNotAuthorized()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var comment = new Comment { Id = commentId, UserId = anotherUserId };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

          
            var result = await _commentService.DeleteCommentAsync(commentId, userId);

           
            Assert.IsFalse(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Never);
        }



      


        [Test]
        public async Task GetAllCommentsAsync_ShouldReturnAllComments()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var comments = new List<Comment>
    {
        new Comment { Id = Guid.NewGuid(), Content = "Comment 1", UserId = userId },
        new Comment { Id = Guid.NewGuid(), Content = "Comment 2", UserId = userId }
    }.AsQueryable();

            var users = new List<ApplicationUser>
    {
        new ApplicationUser { Id = userId, UserName = "TestUser" }
    }.AsQueryable();

            _commentRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(comments.ToList());
            _userRepositoryMock.Setup(repo => repo.GetAllAttached())
                .Returns(users);

            // Act
            var result = await _commentService.GetAllCommentsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Comment 1", result.First().Content);
            
        }

        [Test]
        public async Task GetAllCommentsAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            _commentRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Comment>());

            // Act
            var result = await _commentService.GetAllCommentsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }
        [Test]
        public async Task AdminDeleteCommentAsync_ShouldDeleteComment_WhenCommentExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = new Comment { Id = commentId };

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync(comment);

            _commentRepositoryMock.Setup(repo => repo.DeleteAsync(commentId))
                .ReturnsAsync(true);

            // Act
            var result = await _commentService.AdminDeleteCommentAsync(commentId);

            // Assert
            Assert.IsTrue(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Once);
        }
        [Test]
        public async Task AdminDeleteCommentAsync_ShouldReturnFalse_WhenCommentDoesNotExist()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            _commentRepositoryMock.Setup(repo => repo.GetByIdAsync(commentId))
                .ReturnsAsync((Comment)null);

            // Act
            var result = await _commentService.AdminDeleteCommentAsync(commentId);

            // Assert
            Assert.IsFalse(result);
            _commentRepositoryMock.Verify(repo => repo.DeleteAsync(commentId), Times.Never);
        }





    }
}
