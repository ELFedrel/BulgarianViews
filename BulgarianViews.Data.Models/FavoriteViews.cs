using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Models
{
    [PrimaryKey(nameof(UserId), nameof(LocationId))]
    public class FavoriteViews
    {
        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public Guid LocationId { get; set; }
        [ForeignKey(nameof(LocationId))]
        public LocationPost Location { get; set; } = null!;
    }
}
