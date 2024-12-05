using BulgarianViews.Data.Models;
using BulgarianViews.Web.ViewModels.LocationPost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface ILocationPostService
    {
        Task<List<LocationPostIndexViewModel>> GetAllPostsAsync();
        Task<LocationPostDetailsViewModel> GetPostDetailsAsync(Guid id);
        Task CreatePostAsync(LocationPostCreateViewModel model, Guid userId, string uploadsFolder);
        Task<LocationPostEditViewModel> GetPostForEditAsync(Guid id, Guid userId);
        Task EditPostAsync(LocationPostEditViewModel model, Guid userId, string uploadsFolder);
        Task<LocationPostDeleteViewModel> GetPostForDeleteAsync(Guid id, Guid userId);
        Task DeletePostAsync(LocationPostDeleteViewModel model, Guid userId);
        Task<List<Tag>> GetTagsAsync();
    }
}
