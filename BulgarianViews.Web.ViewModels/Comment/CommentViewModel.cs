using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Comment
{
    using static BulgarianViews.Common.EntityConstants.Comment;
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public Guid UserId { get; set; } 
        public DateTime DateCreated { get; set; }
    }
}
