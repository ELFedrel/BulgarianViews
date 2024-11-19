using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Data.Models
{
    using static BulgarianViews.Common.EntityConstants.Comment;
    public class Comment
    {
        [Key]
        public Guid Id { get; set; } 

        [Required]
        [MinLength(ContentMinLength)]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;



        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;


        [Required]
        public Guid LocationPostId { get; set; }

        [ForeignKey(nameof(LocationPostId))]
        public LocationPost LocationPost { get; set; } = null!;


        [Required]
        public DateTime DateCreated { get; set; }
    }
}
