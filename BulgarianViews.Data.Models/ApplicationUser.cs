using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Models
{
    using static BulgarianViews.Common.EntityConstants.User;
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? ProfilePictureURL { get; set; }


        
        [MaxLength(BioMaxLength)]
        public string? Bio { get; set; }

        public ICollection<LocationPost> LocationPosts  = new List<LocationPost>();
        public ICollection<FavoriteViews> Favorites { get; set; } = new List<FavoriteViews>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
