using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Admin
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public IEnumerable<string> Roles { get; set; } = new List<string>();

    }
}
