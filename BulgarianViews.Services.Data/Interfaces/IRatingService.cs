using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulgarianViews.Services.Data.Interfaces
{
    public interface IRatingService
    {
        Task<bool> RateAsync(Guid postId, Guid userId, int rating);
    }
}
