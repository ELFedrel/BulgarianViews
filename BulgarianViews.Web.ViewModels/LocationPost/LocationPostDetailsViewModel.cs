using BulgarianViews.Web.ViewModels.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.LocationPost
{
    public class LocationPostDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string PhotoURL { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string TagName { get; set; } = null!;

        public List<CommentViewModel> Comments { get; set; } = new();
        public required string PublisherId { get; set; }

        public double AverageRating { get; set; } 



    }
}
