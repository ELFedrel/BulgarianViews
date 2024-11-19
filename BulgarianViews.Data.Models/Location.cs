using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Models
{
    using static BulgarianViews.Common.EntityConstants.Location;
    public class Location
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<LocationPost> LocationPosts { get; set; } = new List<LocationPost>();
    }
}
