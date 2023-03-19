using Dmc.Cms.Model;
using Dmc.Repository;
using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal class RatingRepository : EntityRepositoryBase<Rating>, IRatingRepository
    {
        public RatingRepository(IRepository<Rating> repository) : base(repository)
        {
        }

        public async Task<Rating> FindByUserIdAndPostId(int userId, int postId)
        {
            return await Repository.Query()
                .Filter(o => 
                    o.UserId == userId 
                    && 
                    o.PostId == postId
                )
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Rating>> GetAllUserRatingsAsync(int userId)
        {
            return await Repository.Query()
                .Filter(o =>
                    o.UserId == userId
                )
                .GetEntitiesAsync();
        }

        public async Task<IEnumerable<Rating>> GetAllUserRatingsForPostsAsync(int userId, int[] postIds)
        {
            return await Repository.Query()
                .Filter(o =>
                    o.UserId == userId
                    &&
                    postIds.Contains(o.PostId)
                )
                .GetEntitiesAsync();
        }
    }
}
