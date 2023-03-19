using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IRatingRepository : IEntityRepository<Rating>
    {
        Task<Rating> FindByUserIdAndPostId(int userId, int postId);

        Task<IEnumerable<Rating>> GetAllUserRatingsAsync(int userId);

        Task<IEnumerable<Rating>> GetAllUserRatingsForPostsAsync(int userId, int[] postIds);
    }
}
