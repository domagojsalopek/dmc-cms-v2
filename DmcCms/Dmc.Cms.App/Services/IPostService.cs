using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IPostService : IContentService<Post>, ICrudService<Post> 
    {
        Task<IEnumerable<CommentCountInfo>> CountCommentsForPostIdsAsync(int[] postIds);

        Task<Post> GetNextPostAsync(int currentPostId);

        Task<Post> GetPreviousPostAsync(int currentPostId);

        Task<IEnumerable<Post>> GetLatestbyPostIdsAsync(int[] postIds, int howManyLatest);

        Task<ServiceResult> RatePostAsync(User user, int postId, bool isLike); // we should return result here, so JS knows better!

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<IEnumerable<Category>> GetCategoriesForIdsAsync(int[] categoryIds);

        Task<int> CountPostsInCategoryAsync(int categoryId, bool includeDrafts);

        Task<IEnumerable<Post>> GetPostsInCategoryAsync(int categoryId, int page, int perPage, bool includeDrafts);

        Task<int> CountPostsWithTagAsync(int tagId, bool includeDrafts);

        Task<IEnumerable<Post>> GetPostsWithTagAsync(int tagId, int page, int perPage, bool includeDrafts);

        Task<IEnumerable<Rating>> GetUserRatingsForPostIdsAsync(IUser user, int[] postIds);

        Task<ServiceResult> AddToFavouritesAsync(Guid userId, int postId);

        Task<ServiceResult> RemoveFromFavouritesAsync(Guid userId, int postId);

        Task<IEnumerable<Post>> GetAllByPostIdsAsync(int[] postIds);

        Task<Post> GetRandomPostAsync();
    }
}
