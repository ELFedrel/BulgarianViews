using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Profile
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = null!;
        public string? ProfilePictureURL { get; set; }
        public string? Bio { get; set; }

    }
}
