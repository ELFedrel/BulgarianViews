using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Comment
{
    using static BulgarianViews.Common.EntityConstants.Comment;

    public class CreateCommentViewModel
    {
        [Required]
        [MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;

        public Guid LocationPostId { get; set; }
    }
}
