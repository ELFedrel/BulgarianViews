using BulgarianViews.Web.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileViewModel> GetProfileAsync(Guid userId);
        Task<ProfileEditViewModel> GetProfileForEditAsync(Guid userId);
        Task UpdateProfileAsync(Guid userId, ProfileEditViewModel model);
        Task<List<MyPostViewModel>> GetUserPostsAsync(Guid userId);
        Task<ProfileViewModel> GetUserDetailsAsync(Guid userId);
    }
}
