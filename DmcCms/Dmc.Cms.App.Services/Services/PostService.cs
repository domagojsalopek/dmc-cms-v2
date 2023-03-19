using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.App.Helpers;
using Dmc.Identity;
using Dmc.Cms.App.Identity;

namespace Dmc.Cms.App.Services
{
    // service should return DTOs
    public class PostService : ServiceBase, IPostService
    {
        private readonly ApplicationUserManager _UserManager;

        #region Constructors

        public PostService(ICmsUnitOfWork unitOfWork, ApplicationUserManager userManager) : base(unitOfWork)
        {
            _UserManager = userManager;
        }

        #endregion

        #region Methods

        public Task<Post> GetRandomPostAsync()
        {
            return UnitOfWork.PostRepository.GetRandomPostAsync();
        }

        public Task<IEnumerable<Post>> GetLatestbyPostIdsAsync(int[] postIds, int howManyLatest)
        {
            return UnitOfWork.PostRepository.GetLatestbyPostIdsAsync(postIds, howManyLatest);
        }

        public Task<int> CountAsync()
        {
            return UnitOfWork.PostRepository.CountAsync();
        }

        public Task<int> CountAllPublishedAndDraftsAsync()
        {
            return UnitOfWork.PostRepository.CountPublishedAndDraftsAsync();
        }

        public Task<int> CountAllPublishedAsync()
        {
            return UnitOfWork.PostRepository.CountPublishedAsync();
        }

        public Task<ServiceResult> DeleteAsync(Post entity)
        {
            throw new NotImplementedException();
        }

        public Task<Post> FindBySlugAsync(string slug)
        {
            return UnitOfWork.PostRepository.GetBySlugAsync(slug);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await UnitOfWork.CategoryRepository.GetAllCategoriesAsync();
            return CreateHierarchyFromAllCategories(categories);
        }

        public Task<Post> GetByIdAsync(int id)
        {
            return UnitOfWork.PostRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Category>> GetCategoriesForIdsAsync(int[] categoryIds)
        {
            return UnitOfWork.CategoryRepository.GetCategoriesForIdsAsync(categoryIds);
        }

        public Task<IEnumerable<Post>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.PostRepository.GetPagedAsync(page, perPage);
        }

        public Task<IEnumerable<Post>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            return UnitOfWork.PostRepository.GetPagedPublishedAndDraftsAsync(page, perPage);
        }

        public Task<IEnumerable<Post>> GetPagedPublishedAsync(int page, int perPage)
        {
            return UnitOfWork.PostRepository.GetPagedPublishedAsync(page, perPage);
        }

        public Task<ServiceResult> InsertAsync(Post entity)
        {
            entity.Slug = GeneralUtilities.Slugify(entity.Title);
            UnitOfWork.PostRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Post entity)
        {
            UnitOfWork.PostRepository.Update(entity);
            return SaveAsync();
        }

        public Task<int> CountPostsInCategoryAsync(int categoryId, bool includeDrafts)
        {
            return UnitOfWork.PostRepository.CountInCategory(categoryId, includeDrafts);
        }

        public Task<IEnumerable<Post>> GetPostsInCategoryAsync(int categoryId, int page, int perPage, bool includeDrafts)
        {
            return UnitOfWork.PostRepository.GetPagedInCategoryAsync(categoryId, page, perPage, includeDrafts);
        }

        public Task<int> CountPostsWithTagAsync(int tagId, bool includeDrafts)
        {
            return UnitOfWork.PostRepository.CountPostsWithTagAsync(tagId, includeDrafts);
        }

        public Task<IEnumerable<Post>> GetPostsWithTagAsync(int tagId, int page, int perPage, bool includeDrafts)
        {
            return UnitOfWork.PostRepository.GetPostsWithTagAsync(tagId, page, perPage, includeDrafts);
        }

        public Task<IEnumerable<Rating>> GetUserRatingsForPostIdsAsync(IUser user, int[] postIds)
        {
            return UnitOfWork.RatingRepository.GetAllUserRatingsForPostsAsync(user.Id, postIds);
        }

        public async Task<ServiceResult> AddToFavouritesAsync(Guid userId, int postId)
        {
            User user = await _UserManager.FindUserByUniqueIdAsync(userId);
            
            if (user == null)
            {
                return new ServiceResult("Error.");
            }

            if (user.FavouritePosts == null)
            {
                return new ServiceResult("Error.");
            }

            if (user.FavouritePosts.Any(o => o.Id == postId))
            {
                return ServiceResult.Succeeded;
            }

            Post post = await UnitOfWork.PostRepository.GetByIdAsync(postId);

            if (post == null)
            {
                return new ServiceResult("Error");
            }

            user.FavouritePosts.Add(post);
            return await SaveAsync();
        }

        public async Task<ServiceResult> RemoveFromFavouritesAsync(Guid userId, int postId)
        {
            User user = await _UserManager.FindUserByUniqueIdAsync(userId);

            if (user == null)
            {
                return new ServiceResult("Error.");
            }

            if (user.FavouritePosts == null)
            {
                return new ServiceResult("Error.");
            }

            Post post = user.FavouritePosts.FirstOrDefault(o => o.Id == postId);

            if (post == null)
            {
                return ServiceResult.Succeeded; // already ok
            }

            user.FavouritePosts.Remove(post);
            return await SaveAsync();
        }

        public async Task<IEnumerable<Post>> GetAllByPostIdsAsync(int[] postIds)
        {
            return await UnitOfWork.PostRepository.GetAllByPostIdsAsync(postIds);
        }

        public async Task<ServiceResult> RatePostAsync(User user, int postId, bool isLikeAction)
        {
            var existingRating = await UnitOfWork.RatingRepository.FindByUserIdAndPostId(user.Id, postId);

            if (existingRating == null)
            {
                return await CreateAndSaveNewRating(user, postId, isLikeAction);
            }

            if (IsUndo(existingRating, isLikeAction))
            {
                UnitOfWork.RatingRepository.Delete(existingRating);
                return await SaveAsync();
            }

            existingRating.IsLike = isLikeAction; // is this OK?
            UnitOfWork.RatingRepository.Update(existingRating);
            return await SaveAsync();
        }

        private bool DetermineRatingToSet(Rating existingRating, bool isLikeAction)
        {
            if (isLikeAction && !existingRating.IsLike)
            {
                return true;
            }

            if (!isLikeAction && existingRating.IsLike)
            {
                return false;
            }

            return isLikeAction;
        }

        private bool IsUndo(Rating existingRating, bool isLikeAction)
        {
            if (existingRating.IsLike && isLikeAction)
            {
                return true;
            }

            if (!existingRating.IsLike && !isLikeAction)
            {
                return true;
            }

            return false;
        }

        public async Task<Post> FindByUniqueIdAsync(Guid uniqueId)
        {
            return await UnitOfWork.PostRepository.GetByUniqueIdAsync(uniqueId);
        }

        public async Task<Post> GetNextPostAsync(int currentPostId)
        {
            return await UnitOfWork.PostRepository.GetNextPostAsync(currentPostId);
        }

        public async Task<Post> GetPreviousPostAsync(int currentPostId)
        {
            return await UnitOfWork.PostRepository.GetPreviousPostAsync(currentPostId);
        }

        #endregion

        #region Private Methods

        private async Task<ServiceResult> CreateAndSaveNewRating(IUser user, int postId, bool isLike)
        {
            Rating newRating = new Rating
            {
                IsLike = isLike,
                PostId = postId,
                UserId = user.Id
            };

            UnitOfWork.RatingRepository.Insert(newRating);
            return await SaveAsync();
        }

        public Task<IEnumerable<CommentCountInfo>> CountCommentsForPostIdsAsync(int[] postIds)
        {
            return UnitOfWork.PostRepository.CountCommentsForPostIdsAsync(postIds);
        }

        #endregion
    }
}
