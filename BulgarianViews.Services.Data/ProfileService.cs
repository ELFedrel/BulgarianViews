using BulgarianViews.Data;
using BulgarianViews.Services.Data.Interfaces;
using BulgarianViews.Web.ViewModels.Profile;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data
{
    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _context;

        public ProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProfileViewModel> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new ProfileViewModel
            {
                UserName = user.UserName,
                ProfilePictureURL = user.ProfilePictureURL,
                Bio = user.Bio
            };
        }

        public async Task<ProfileEditViewModel> GetProfileForEditAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new ProfileEditViewModel
            {
                UserName = user.UserName,
                Bio = user.Bio,
                ProfilePictureURL = user.ProfilePictureURL
            };
        }

        public async Task UpdateProfileAsync(Guid userId, ProfileEditViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.UserName = model.UserName;
            user.Bio = model.Bio;

            if (model.ProfilePicture != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var uniqueFileName = $"{Guid.NewGuid()}_{model.ProfilePicture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                user.ProfilePictureURL = $"/images/{uniqueFileName}";
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MyPostViewModel>> GetUserPostsAsync(Guid userId)
        {
            var posts = await _context.LocationPosts
                .Where(lp => lp.UserId == userId)
                .Select(lp => new MyPostViewModel
                {
                    Id = lp.Id,
                    Title = lp.Title,
                    Description = lp.Description,
                    PhotoURL = lp.PhotoURL,
                    AverageRating = lp.AverageRating
                })
                .ToListAsync();

            return posts;
        }

        public async Task<ProfileViewModel> GetUserDetailsAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return new ProfileViewModel
            {
                UserName = user.UserName,
                ProfilePictureURL = user.ProfilePictureURL ?? "/images/default-profile.png",
                Bio = user.Bio
            };
        }
    }
}
