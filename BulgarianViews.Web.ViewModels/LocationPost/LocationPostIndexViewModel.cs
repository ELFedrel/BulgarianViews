using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.LocationPost
{
    public class LocationPostIndexViewModel
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string PhotoURL { get; set; }
       
        public required string UserName { get; set; }
        public required string PublisherId { get; set; }
      







    }
}
