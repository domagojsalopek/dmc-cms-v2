using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IPostRepository : IContentRepository<Post>
    {
        Task<IEnumerable<CommentCountInfo>> CountCommentsForPostIdsAsync(int[] postIds);

        Task<Post> GetNextPostAsync(int currentPostId);

        Task<Post> GetPreviousPostAsync(int currentPostId);

        Task<IEnumerable<Post>> GetLatestbyPostIdsAsync(int[] postIds, int howManyLatest);

        Task<int> CountInCategory(int categoryId, bool includeDrafts);

        Task<IEnumerable<Post>> GetPagedInCategoryAsync(int categoryId, int page, int perPage, bool includeDrafts);

        Task<int> CountPostsWithTagAsync(int tagId, bool includeDrafts);

        Task<IEnumerable<Post>> GetPostsWithTagAsync(int tagId, int page, int perPage, bool includeDrafts);

        Task<int> CountSearchAsync(string[] search);

        Task<IEnumerable<Post>> SearchPostsAsync(string[] search, int page, int perPage);

        Task<IEnumerable<Post>> GetAllByPostIdsAsync(int[] postIds);

        Task<Post> GetRandomPostAsync();
    }
}
