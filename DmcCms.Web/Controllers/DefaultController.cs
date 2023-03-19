using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Cms.Repository.Ef;
using Dmc.Cms.Web.Mappers;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class DefaultController : FrontEndControllerBase<Post, PostViewModel>
    {
        #region Constants

        private const string FavoritePostsCacheKeyPrefix = "FavoritePosts";
        private const string SidebarCategoriesCacheKey = "SidebarCategories";
        private const string RecentPostsCacheKey = "RecentPosts";
        private const string TagsCacheKey = "Tags";
        private const string IndexCacheKeyPrefix = "IndexPosts";
        private const string DetailsCacheKeyPrefix = "PostDetails";

        private const string CategoryPostsPrefix = "CategoryPosts";
        private const string TagPostsPrefix = "TagPosts";

        private const int ListExpirationCacheInMinutes = 30;
        private const int PostDetailsCacheExpirationInMinutes = 60; // not to have too much in cache

        #endregion

        #region Private Fields

        private Category _Category;
        private Tag _Tag;
        private readonly IPostService _PostService;
        private readonly ICategoryService _CategoryService;
        private readonly ITagService _TagService;
        private bool? _CanUserSeeDrafts = null;

        #endregion

        #region Constructors

        public DefaultController(IPostService service, ICategoryService categoryService, ITagService tagService, ApplicationUserManager userManager) 
            : base(service, userManager)
        {
            _PostService = service ?? throw new ArgumentNullException(nameof(service));
            _CategoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _TagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        }

        #endregion

        #region Properties

        public bool DraftsVisible
        {
            get
            {
                if (_CanUserSeeDrafts.HasValue)
                {
                    return _CanUserSeeDrafts.Value;
                }

                _CanUserSeeDrafts = ResolveIfUserCanSeeDrafts();
                return _CanUserSeeDrafts.Value;
            }
        }

        #endregion

        #region Web Methods

        public async Task<ActionResult> Index(int? page, int? perPage)
        {
            string cacheKey = string.Format("{0}_{1}_{2}_Drafts_{3}"
                , IndexCacheKeyPrefix
                , page.GetValueOrDefault()
                , perPage ?? AppConfig.SiteSettings.PostsPerPage
                , DraftsVisible);

            EntityList<PostViewModel> results = CacheHelper.GetFromCache<EntityList<PostViewModel>>(cacheKey); // try to get from cache
            if (results == null)
            {
                results = await GetEntityListAsync(page, perPage); // try get from DB
            }

            if (results != null && results.TotalCount > 0)
            {
                CacheHelper.AddToCache(cacheKey, results, DateTimeOffset.UtcNow.AddMinutes(ListExpirationCacheInMinutes));
            }

            // we need to do this, otherwise those in cache would be changed by filling with user info.
            EntityList<PostViewModel> resultsToReturn = CreateAndFillNewResults(results);

            // Append count info
            await AppendMetaData(resultsToReturn);

            if (User.Identity.IsAuthenticated)
            {
                await FillViewModelWithUserInfo(resultsToReturn);
            }

            return ListView(resultsToReturn);
        }

        private async Task AppendMetaData(EntityList<PostViewModel> results)
        {
            IEnumerable<CommentCountInfo> commentCountInfos = await _PostService.CountCommentsForPostIdsAsync(results.Results.Select(o => o.Id.Value).ToArray());

            foreach (CommentCountInfo commentCountInfo in commentCountInfos)
            {
                var post = results.Results.FirstOrDefault(o => o.Id == commentCountInfo.OwnerId);

                if (post == null)
                {
                    continue;
                }

                post.NumberOfComments = commentCountInfo.NumberOfComments;
            }
        }

        public async Task<ActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            string cacheKey = string.Format("{0}_{1}_Drafts_{2}"
                , DetailsCacheKeyPrefix
                , slug.ToUpper()
                , DraftsVisible); // those who cannot see drafts won't find in the cache. they will try to get from db and get not found, so it won't be ever added to cache

            PostViewModel detailsViewModel = CacheHelper.GetFromCache<PostViewModel>(cacheKey);
            if (detailsViewModel == null)
            {
                detailsViewModel = await GetEntityAndLoadViewModelAsync(slug);
            }            

            if (detailsViewModel == null)
            {
                return NotFound();
            }

            // add to cache as we know it's not null here.
            // we add before filling with user info as we don't yet have the cache for ratings etc.
            // we also don't have statistics etc yet. everything appended is per user
            CacheHelper.AddToCache(cacheKey, detailsViewModel, DateTimeOffset.UtcNow.AddMinutes(PostDetailsCacheExpirationInMinutes));

            // we need to create new in order to append rating etc things to that. in order not to change one in cache
            PostViewModel resultToReturn = new PostViewModel
            {
                Categories = detailsViewModel.Categories,
                Content = detailsViewModel.Content,
                Description = detailsViewModel.Description,
                DetailImage = detailsViewModel.DetailImage,
                HasRating = false,
                Id = detailsViewModel.Id,
                IsCommentingEnabled = detailsViewModel.IsCommentingEnabled,
                IsFavourite = false,
                Liked = false,
                PreviewImage = detailsViewModel.PreviewImage,
                Published = detailsViewModel.Published,
                Slug = detailsViewModel.Slug,
                Status = detailsViewModel.Status,
                Tags = detailsViewModel.Tags,
                Title = detailsViewModel.Title,
                NextPost = detailsViewModel.NextPost,
                PreviousPost = detailsViewModel.PreviousPost,
                NumberOfComments = detailsViewModel.NumberOfComments
            };

            if (User.Identity.IsAuthenticated)
            {
                await FillViewModelWithUserInfo(new EntityList<PostViewModel>(new List<PostViewModel> { resultToReturn }, 1, 1, 1));
            }

            // we don't count comments again as in the details commets are eagerly loaded
            // but since we switched to knockout comments with ajax we should change all this

            return DetailView(resultToReturn);
        }

        protected override async Task<PostViewModel> GetEntityAndLoadViewModelAsync(string slug)
        {
            var viewModel = await base.GetEntityAndLoadViewModelAsync(slug);

            if (viewModel == null)
            {
                return null;
            }

            if (!viewModel.Id.HasValue)
            {
                return viewModel;
            }

            var previous = await _PostService.GetPreviousPostAsync(viewModel.Id.Value);
            var next = await _PostService.GetNextPostAsync(viewModel.Id.Value);

            if (previous != null)
            {
                viewModel.PreviousPost = new PostInfoViewModel
                {
                    Id = previous.Id, 
                    Slug = previous.Slug,
                    Title = previous.Title
                };
            }

            if (next != null)
            {
                viewModel.NextPost = new PostInfoViewModel
                {
                    Id = next.Id,
                    Slug = next.Slug,
                    Title = next.Title
                };
            }

            return viewModel;
        }

        public async Task<ActionResult> PostsByCategory(string categoryName, int? page, int? perPage)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return NotFound();
            }

            string cacheKey = string.Format("{0}_{1}_{2}_{3}_Drafts_{4}"
                , CategoryPostsPrefix
                , categoryName.ToUpper()
                , page
                , perPage ?? AppConfig.SiteSettings.PostsPerPage
                , DraftsVisible);

            EntityList<PostViewModel> results = CacheHelper.GetFromCache<EntityList<PostViewModel>>(cacheKey);

            // we need it always. todo: make it so that we don't
            _Category = await _CategoryService.FindBySlugAsync(categoryName);

            if (_Category == null || !IsEntityDisplayable(_Category))
            {
                return NotFound();
            }

            if (results == null)
            {
                results = await GetEntityListAsync(page, perPage);
            }
            
            if (results != null && results.TotalCount > 0)
            {
                CacheHelper.AddToCache(cacheKey, results, DateTimeOffset.UtcNow.AddMinutes(ListExpirationCacheInMinutes));
            }

            // we need to do this, otherwise those in cache would be changed by filling with user info.
            EntityList<PostViewModel> resultsToReturn = CreateAndFillNewResults(results);

            // Append count info
            await AppendMetaData(resultsToReturn);

            if (User.Identity.IsAuthenticated)
            {
                await FillViewModelWithUserInfo(resultsToReturn);
            }

            CategoryViewModel viewModel = CreateCategoryViewModel(_Category, resultsToReturn);
            return View(viewModel);
        }

        public async Task<ActionResult> PostsByTag(string tagName, int? page, int? perPage)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return NotFound();
            }

            string cacheKey = string.Format("{0}_{1}_{2}_{3}_CanSeeDrafts_{4}"
                , TagPostsPrefix
                , tagName.ToUpper()
                , page
                , perPage ?? AppConfig.SiteSettings.PostsPerPage
                , DraftsVisible);

            _Tag = await _TagService.FindBySlugAsync(tagName);

            if (_Tag == null || !IsEntityDisplayable(_Tag))
            {
                return NotFound();
            }

            EntityList<PostViewModel> results = CacheHelper.GetFromCache<EntityList<PostViewModel>>(cacheKey);
            if (results == null)
            {
                results = await GetEntityListAsync(page, perPage);
            }

            if (results != null && results.TotalCount > 0)
            {
                CacheHelper.AddToCache(cacheKey, results, DateTimeOffset.UtcNow.AddMinutes(ListExpirationCacheInMinutes));
            }

            // we need to do this, otherwise those in cache would be changed by filling with user info.
            EntityList<PostViewModel> resultsToReturn = CreateAndFillNewResults(results);

            // Append count info
            await AppendMetaData(resultsToReturn);

            if (User.Identity.IsAuthenticated)
            {
                await FillViewModelWithUserInfo(resultsToReturn);
            }

            TagViewModel viewModel = new TagViewModel
            {
                Title = _Tag.Title,
                Posts = resultsToReturn,
                Published = _Tag.Published,
                Slug = _Tag.Slug,
                Status = _Tag.Status,
                Id = _Tag.Id
            };

            return View(viewModel);
        }

        #endregion

        #region Abstract Implementation Methods

        internal override void TransferFromEntityToBrowseViewModel(Post entity, PostViewModel viewModel)
        {
            PostMapper.TransferToViewModel(entity, viewModel);
        }

        internal override void TransferFromEntityToDetailsViewModel(Post entity, PostViewModel detailsViewModel)
        {
            PostMapper.TransferToViewModel(entity, detailsViewModel);
        }

        #endregion

        #region Override Methods

        //TODO: maybe different controllers for tag display, etc ... 

        protected override async Task<int> CountPublishedAndDraftsAsync(IDictionary<string, object> routeData)
        {
            if (_Tag != null)
            {
                return await _PostService.CountPostsWithTagAsync(_Tag.Id, true);
            }

            if (_Category != null)
            {
                return await _PostService.CountPostsInCategoryAsync(_Category.Id, true);
            }

            return await _PostService.CountAllPublishedAndDraftsAsync();
        }

        protected override async Task<int> CountPublishedAsync(IDictionary<string, object> routeData)
        {
            if (_Tag != null)
            {
                return await _PostService.CountPostsWithTagAsync(_Tag.Id, false);
            }

            if (_Category != null)
            {
                return await _PostService.CountPostsInCategoryAsync(_Category.Id, false);
            }

            return await _PostService.CountAllPublishedAsync();
        }

        protected override async Task<IEnumerable<Post>> GetPagedPublishedAndDraftsAsync(int page, int perPage, IDictionary<string, object> routeData)
        {
            if (_Tag != null)
            {
                return await _PostService.GetPostsWithTagAsync(_Tag.Id, page, perPage, true);
            }

            if (_Category != null)
            {
                return await _PostService.GetPostsInCategoryAsync(_Category.Id, page, perPage, true); 
            }

            return await base.GetPagedPublishedAndDraftsAsync(page, perPage, routeData);
        }

        protected override async Task<IEnumerable<Post>> GetPagedPublishedAsync(int page, int perPage, IDictionary<string, object> routeData)
        {
            if (_Tag != null)
            {
                return await _PostService.GetPostsWithTagAsync(_Tag.Id, page, perPage, false);
            }

            if (_Category != null)
            {
                return await _PostService.GetPostsInCategoryAsync(_Category.Id, page, perPage, false);
            }

            return await base.GetPagedPublishedAsync(page, perPage, routeData);
        }

        #endregion

        #region Private Methods

        private CategoryViewModel CreateCategoryViewModel(Category category, EntityList<PostViewModel> entityList)
        {
            CategoryViewModel result = new CategoryViewModel
            {
                Posts = entityList
            };

            result.Description = category.Description;
            result.Title = category.Title;
            result.Slug = category.Slug;
            result.Status = category.Status;
            result.Id = category.Id;

            if (_Category.IntroImage != null)
            {
                result.IntroImage = new ImageViewModel();
                ImageMapper.TransferToViewModel(_Category.IntroImage, result.IntroImage);
            }

            return result;
        }

        #endregion

        #region Private Additional Methods

        private EntityList<PostViewModel> CreateAndFillNewResults(EntityList<PostViewModel> results)
        {
            List<PostViewModel> innerList = results.Results.Select(o => new PostViewModel
            {
                Categories = o.Categories,
                Content = o.Content,
                Description = o.Description,
                DetailImage = o.DetailImage,
                HasRating = o.HasRating,
                Id = o.Id,
                IsCommentingEnabled = o.IsCommentingEnabled,
                IsFavourite = o.IsFavourite,
                Liked = o.Liked,
                PreviewImage = o.PreviewImage,
                Published = o.Published,
                Slug = o.Slug,
                Status = o.Status,
                Tags = o.Tags,
                Title = o.Title
            })
            .ToList(); // we need to say to list, otherwise we're working with deffered and won't be changing them as yield will return the new object
            return new EntityList<PostViewModel>(innerList, results.PageIndex, results.PageSize, results.TotalCount);
        }

        private async Task FillViewModelWithUserInfo(EntityList<PostViewModel> entityList)
        {
            User loggedInUser = await GetLoggedInUserAsync(); 

            if (loggedInUser == null)
            {
                return;
            }

            int[] postIds = entityList.Results.Where(o => o.Id.HasValue).Select(o => o.Id.Value).ToArray();
            IEnumerable<Rating> ratingsForCurrentUserForThesePosts = await _PostService.GetUserRatingsForPostIdsAsync(loggedInUser, postIds);

            foreach (var item in entityList.Results)
            {
                AppendRatingAndFavouriteInfo(item, ratingsForCurrentUserForThesePosts, loggedInUser.FavouritePosts);
            }
        }

        private void AppendRatingAndFavouriteInfo(PostViewModel item, IEnumerable<Rating> ratingsForCurrentUserForThesePosts, ICollection<Post> favouritePosts)
        {
            Rating userRatingForTost = ratingsForCurrentUserForThesePosts.FirstOrDefault(o => o.PostId == item.Id);

            if (userRatingForTost != null)
            {
                item.HasRating = true;
                item.Liked = userRatingForTost.IsLike;
            }

            item.IsFavourite = favouritePosts.Any(o => o.Id == item.Id);
        }

        #endregion

        #region Ajax Helper Methods

        [Authorize]
        public async Task<PartialViewResult> GetFavouritePosts()
        {
            if (!Request.IsAjaxRequest() || !User.Identity.IsAuthenticated)
            {
                return null;
            }

            User user = await GetLoggedInUserAsync();
            if (user == null)
            {
                return null;
            }

            string cacheKey = CreateCacheKey(FavoritePostsCacheKeyPrefix, user);
            List<PostViewModel> resultsFromCache = CacheHelper.GetFromCache<List<PostViewModel>>(cacheKey);
            if (resultsFromCache != null)
            {
                return PartialView("Partials/_FavouritePostsPartial", resultsFromCache);
            }

            var results = new List<PostViewModel>();
            int[] postsIds = user.FavouritePosts.Select(o => o.Id).ToArray();
            IEnumerable<Post> latestFavouritePosts = await _PostService.GetLatestbyPostIdsAsync(postsIds, AppConfig.SiteSettings.LatestUserFavouritePosts);
            foreach (var item in latestFavouritePosts)
            {
                PostViewModel viewModel = new PostViewModel();
                TransferFromEntityToBrowseViewModel(item, viewModel);

                results.Add(viewModel);
            }

            AddResultsToCacheIfEligible(cacheKey, results);
            return PartialView("Partials/_FavouritePostsPartial", results);
        }

        public async Task<PartialViewResult> GetCategoryList()
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            List<CategoryWithSubcategoriesListViewModel> resultsFromCache = CacheHelper.GetFromCache<List<CategoryWithSubcategoriesListViewModel>>(SidebarCategoriesCacheKey);

            if (resultsFromCache != null)
            {
                return PartialView("Partials/_SidebarCategoriesPartial", resultsFromCache);
            }

            List<CategoryWithSubcategoriesListViewModel> allCategories = await GetCategoryListAsync();

            if (allCategories != null && allCategories.Count > 0)
            {
                CacheHelper.AddToCache(SidebarCategoriesCacheKey, allCategories);
            }

            return PartialView("Partials/_SidebarCategoriesPartial", allCategories);
        }

        public async Task<PartialViewResult> GetRecentPosts()
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            List<PostViewModel> viewModelListFromCache = CacheHelper.GetFromCache<List<PostViewModel>>(RecentPostsCacheKey);
            if (viewModelListFromCache != null)
            {
                return PartialView("Partials/_RecentPostsPartial", viewModelListFromCache);
            }

            IEnumerable<Post> recentPoosts = await _PostService.GetPagedAsync(1, AppConfig.SiteSettings.NumberOfRecentPostsToShow);
            List<PostViewModel> viewModelList = new List<PostViewModel>();
            foreach (var item in recentPoosts)
            {
                PostViewModel viewModel = new PostViewModel();
                TransferFromEntityToBrowseViewModel(item, viewModel);

                viewModelList.Add(viewModel);
            }

            if (viewModelList.Count > 0)
            {
                CacheHelper.AddToCache(RecentPostsCacheKey, viewModelList);
            }

            return PartialView("Partials/_RecentPostsPartial", viewModelList);
        }

        public async Task<PartialViewResult> GetTagList()
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            List<TagListViewModel> tagsFromCache = CacheHelper.GetFromCache<List<TagListViewModel>>(TagsCacheKey);
            if (tagsFromCache != null)
            {
                return PartialView("Partials/_SidebarTagsPartial", tagsFromCache);
            }

            List<TagListViewModel> tags = await GetTagsListAsync();

            if (tags != null && tags.Count > 0)
            {
                CacheHelper.AddToCache(TagsCacheKey, tags);
            }

            return PartialView("Partials/_SidebarTagsPartial", tags);
        }

        #endregion

        #region Private Ajax Helper Methods

        private async Task<List<CategoryWithSubcategoriesListViewModel>> GetCategoryListAsync()
        {
            IEnumerable<Category> categories = await _CategoryService.GetCategoriesWithPostsAsync(); // service already sorts it
            IEnumerable<PostCountInfo> postCountInfos = await _CategoryService.CountPostInCategoriesAsync(categories.Select(o => o.Id).ToArray());

            List<CategoryWithSubcategoriesListViewModel> result = new List<CategoryWithSubcategoriesListViewModel>();

            if (categories == null)
            {
                return result;
            }

            FillCategorySelectList(result, categories, postCountInfos);
            return result;
        }

        protected void FillCategorySelectList(List<CategoryWithSubcategoriesListViewModel> categorySelectList, IEnumerable<Category> allCategories, IEnumerable<PostCountInfo> postCountInfos)
        {
            var rootCategories = allCategories.Where(o => o.ParentId == null);

            foreach (Category rootCategory in rootCategories)
            {
                var viewModel = new CategoryWithSubcategoriesListViewModel
                {
                    Name = rootCategory.Title,
                    Slug = rootCategory.Slug
                };

                PostCountInfo postCountInfo = postCountInfos.Where(o => o.OwnerId == rootCategory.Id).FirstOrDefault();

                if (postCountInfo != null)
                {
                    viewModel.NumberOfPosts = postCountInfo.NumberOfPosts;
                }

                if (rootCategory.HasChildren)
                {
                    AppendChildren(viewModel.Subcategories, rootCategory.Children, postCountInfos); // children of root are on level 1
                }

                categorySelectList.Add(viewModel);
            }
        }

        private void AppendChildren(List<CategoryWithSubcategoriesListViewModel> subcategories, ICollection<Category> children, IEnumerable<PostCountInfo> postCountInfos)
        {
            foreach (var item in children)
            {
                var viewModel = new CategoryWithSubcategoriesListViewModel
                {
                    Name = item.Title,
                    Slug = item.Slug
                };

                PostCountInfo postCountInfo = postCountInfos.Where(o => o.OwnerId == item.Id).FirstOrDefault();

                if (postCountInfo != null)
                {
                    viewModel.NumberOfPosts = postCountInfo.NumberOfPosts;
                }

                if (item.HasChildren)
                {
                    AppendChildren(viewModel.Subcategories, item.Children, postCountInfos);
                }

                subcategories.Add(viewModel);
            }
        }

        private async Task<List<TagListViewModel>> GetTagsListAsync()
        {
            var tags = await _TagService.GetUsedTagsAsync();

            List<TagListViewModel> result = new List<TagListViewModel>();

            if (tags == null)
            {
                return result;
            }

            foreach (var item in tags)
            {
                result.Add(new TagListViewModel
                {
                    Name = item.Title,
                    Slug = item.Slug
                });
            }

            return result;
        }

        #endregion

        #region Private Methods

        private void AddResultsToCacheIfEligible(string cacheKey, List<PostViewModel> viewModelList)
        {
            if (viewModelList == null || viewModelList.Count <= 0)
            {
                return;
            }

            CacheHelper.AddToCache(cacheKey, viewModelList); // never expires?
        }

        private static string CreateCacheKey(string cacheKeyPrefix, User user)
        {
            return string.Format("{0}_User_{1}", cacheKeyPrefix, user.UniqueId);
        }

        private bool ResolveIfUserCanSeeDrafts()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            return LoggedInUserId.HasValue && CanUserSeeDrafts(LoggedInUserId.Value, User.Identity as ClaimsIdentity);
        }

        #endregion
    }
}