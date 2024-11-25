using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Models
{
    using static BulgarianViews.Common.EntityConstants.Rating;

    public class Rating
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [Range(MinRating, MaxRating)]
        public int Value { get; set; }

        
        [Required]
        public Guid LocationPostId { get; set; }

        [ForeignKey(nameof(LocationPostId))]
        public LocationPost LocationPost { get; set; } = null!;

        
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;
    }
}
