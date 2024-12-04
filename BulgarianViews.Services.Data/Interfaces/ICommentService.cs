using BulgarianViews.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(CreateCommentViewModel model, Guid userId);
        Task<IEnumerable<CommentViewModel>> GetCommentsByPostIdAsync(Guid postId);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid userId);
    }
}
