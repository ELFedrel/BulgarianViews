using BulgarianViews.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public string Title { get; set; } = null!;

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "Please upload a photo.")]
        public IFormFile PhotoURL { get; set; } = null!;

        [Required(ErrorMessage = "Please select a tag.")]
        public Guid TagsId { get; set; }

        public IEnumerable<SelectListItem> Tags { get; set; } = new List<SelectListItem>();





    }
}
