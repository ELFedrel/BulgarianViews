using BulgarianViews.Data.Models;
using BulgarianViews.Data.Repositories.Interfaces;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment, Guid> _commentRepository;
        private readonly IRepository<ApplicationUser, Guid> _userRepository;

        public CommentService(IRepository<Comment, Guid> commentRepository, IRepository<ApplicationUser, Guid> userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public async Task AddCommentAsync(CreateCommentViewModel model, Guid userId)
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = model.Content,
                UserId = userId,
                LocationPostId = model.LocationPostId,
                DateCreated = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
        }

        public async Task<IEnumerable<CommentViewModel>> GetCommentsByPostIdAsync(Guid postId)
        {
            var comments = await _commentRepository
                .FindAsync(c => c.LocationPostId == postId);

            return comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Content = c.Content,
                UserName = _userRepository.GetById(c.UserId)?.UserName ?? "Unknown",
                DateCreated = c.DateCreated
            });
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                return false;
            }

            return await _commentRepository.DeleteAsync(commentId);
        }
    }
}
