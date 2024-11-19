using BulgarianViews.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.LocationPost
{
    using static BulgarianViews.Common.EntityConstants.LocationPost;    
    public class LocationPostCreateViewModel
    {
        [Required]
        [MinLength(TitleMinLength)]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        
        public IFormFile PhotoURL { get; set; }


        [Required]
        public Guid TagsId { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();





        
    }
}
