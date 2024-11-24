using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Profile
{
    using static BulgarianViews.Common.EntityConstants.User;


    public class ProfileEditViewModel
    {
        [Required]
        [MinLength(UserNameMinLength)]
        [MaxLength(UserNameMaxLength)]
        public string UserName { get; set; } = null!;
        [Required]
        [MaxLength(BioMaxLength)]
        [MinLength(BioMinLength)]
        public string? Bio { get; set; }


        
        public IFormFile? ProfilePicture { get; set; }

        
        public string? ProfilePictureURL { get; set; }



    }
}
