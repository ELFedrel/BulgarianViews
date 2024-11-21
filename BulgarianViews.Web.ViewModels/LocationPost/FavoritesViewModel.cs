﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Web.ViewModels.LocationPost
{
    public class FavoritesViewModel
    {
        public Guid Id { get; set; } 
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PhotoURL { get; set; } = null!;
    }
}
