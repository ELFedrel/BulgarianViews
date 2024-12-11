using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Admin
{
    public class ManagePostViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PublisherName { get; set; } = null!;
        public double AverageRating { get; set; }
        public string PhotoURL { get; set; } = null!;
        
        public Guid UserId { get; set; }
    }
}
