using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.Admin
{
    public class EditUserViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> AllRoles { get; set; } = new List<string>();
    }
}
