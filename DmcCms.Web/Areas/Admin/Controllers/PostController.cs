using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.Web.ViewModels;
using System.Threading.Tasks;
using Dmc.Identity;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App;
using System.Web.Hosting;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class PostController : CRUDControllerBase<Post> //TODO: use same view model ... TODO: test also with category service injection !!
    {
        #region Fields

        private readonly ICategoryService _CategoryService;
        private readonly IPostService _PostService;
        private readonly ITagService _TagService;
        private readonly IImageService _ImageService;
        private readonly ISitemapService _SitemapService;
        private readonly IIdentityUnitOfWork<User> _IdentityUnitOfWork; // should not use unit of work directly ... !!

        #endregion

        #region Constructors

        public PostController(IPostService service, ITagService tagService, IImageService imageService, ISitemapService sitemapService, IIdentityUnitOfWork<User> unitOfWork, ApplicationUserManager userManager) 
            : base(service, userManager)
        {
            _PostService = service ?? throw new ArgumentNullException(nameof(service));
            _TagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
            _ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _SitemapService = sitemapService ?? throw new ArgumentNullException(nameof(sitemapService));
            _IdentityUnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminPostViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminPostViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Implementation

        internal override ICrudViewModel CreateBrowseViewModel(Post entity)
        {
            AdminPostViewModel result = new AdminPostViewModel();

            result.Title = entity.Title;
            result.Description = entity.Description;
            result.Status = entity.Status;
            result.Slug = entity.Slug;
            result.Content = entity.Content;
            result.Published = entity.Published;

            return result;
        }

        internal override async Task<Post> CreateEntityFromViewModelAsync(ICrudViewModel viewModel)
        {
            AdminPostViewModel vm = viewModel as AdminPostViewModel;

            var categories = await _PostService.GetCategoriesForIdsAsync(vm.SelectedCategories.Where(o => o.HasValue).Select(o => o.Value).ToArray());
            var tags = vm.SelectedTags != null
                ? await _TagService.GetTagsForIdsAsync(vm.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray())
                : new List<Tag>();

            Post post = new Post();
            TransferFromViewModelToModel(vm, categories, tags, post);

            post.PreviewImage = vm.PreviewImageId.HasValue ? await _ImageService.GetByIdAsync(vm.PreviewImageId.Value) : null;
            post.DetailImage = vm.DetailImageId.HasValue ? await _ImageService.GetByIdAsync(vm.DetailImageId.Value) : null;

            if (User.Identity.IsAuthenticated)
            {
                Guid? loggedInUserId = GetLoggedInUserId();
                post.Author = await _IdentityUnitOfWork.UserRepository
                    .Query()
                    .Filter(o => o.UniqueId == loggedInUserId)
                    .FirstOrDefaultAsync();
            }

            return post;
        }

        internal override async Task<ICrudViewModel> CreateViewModelAsync()
        {
            AdminPostViewModel result = new AdminPostViewModel();

            var allCategories = await _PostService.GetAllCategoriesAsync();
            var allTags = await _TagService.GetAllTagsAsync();
            var allImages = await _ImageService.GetAllImagesAsync();
            result.Categories = GetCategoriesSelectList(allCategories, null);
            result.Tags = GetTagsSelectList(allTags, null);
            result.Images = GetImagesSelectList(allImages);

            return result;
        }

        internal override async Task OperationFailedFillViewModelAsync(ICrudViewModel viewModel, Post entity)
        {
            AdminPostViewModel categoryViewModel = viewModel as AdminPostViewModel;

            if (categoryViewModel != null)
            {
                var allCategories = await _PostService.GetAllCategoriesAsync();
                var allTags = await _TagService.GetAllTagsAsync();
                var allImages = await _ImageService.GetAllImagesAsync();
                categoryViewModel.Categories = GetCategoriesSelectList(allCategories, categoryViewModel.SelectedCategories.Where(o => o.HasValue).Select(o => o.Value).ToArray());

                var selectedTags = categoryViewModel.SelectedTags != null
                    ? categoryViewModel.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray()
                    : new int[0];

                categoryViewModel.Tags = GetTagsSelectList(allTags, selectedTags);
                categoryViewModel.Images = GetImagesSelectList(allImages);
            }
        }

        internal override async Task TransferDataFromEntityToViewModelAsync(Post entity, ICrudViewModel viewModel)
        {
            AdminPostViewModel result = viewModel as AdminPostViewModel;

            // first set selected
            TransferDataToViewModel(entity, result);

            // then fill dropdown
            var allCategories = await _PostService.GetAllCategoriesAsync();
            var allTags = await _TagService.GetAllTagsAsync();
            var allImages = await _ImageService.GetAllImagesAsync();

            var selectedTags = result.SelectedTags != null
                    ? result.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray()
                    : new int[0];

            result.Categories = GetCategoriesSelectList(allCategories, result.SelectedCategories.Where(o => o.HasValue).Select(o => o.Value).ToArray());
            result.Tags = GetTagsSelectList(allTags, selectedTags);
            result.Images = GetImagesSelectList(allImages);

            return;
        }

        internal override async Task TransferDataFromViewModelToEntityAsync(ICrudViewModel viewModel, Post entity)
        {
            AdminPostViewModel vm = viewModel as AdminPostViewModel;
            var categories = await _PostService.GetCategoriesForIdsAsync(vm.SelectedCategories.Where(o => o.HasValue).Select(o => o.Value).ToArray());

            //var tags = await _TagService.GetTagsForIdsAsync(vm.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray());
            var tags = vm.SelectedTags != null
                ? await _TagService.GetTagsForIdsAsync(vm.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray())
                : new List<Tag>();

            TransferFromViewModelToModel(vm, categories, tags, entity);

            entity.PreviewImage = vm.PreviewImageId.HasValue ? await _ImageService.GetByIdAsync(vm.PreviewImageId.Value) : null;
            entity.DetailImage = vm.DetailImageId.HasValue ? await _ImageService.GetByIdAsync(vm.DetailImageId.Value) : null;

            if (User.Identity.IsAuthenticated)
            {
                Guid? loggedInUserId = GetLoggedInUserId();
                entity.Author = await _IdentityUnitOfWork.UserRepository
                    .Query()
                    .Filter(o => o.UniqueId == loggedInUserId)
                    .FirstOrDefaultAsync();
            }
        }

        internal override async Task ValidationFailedFillViewModelIfNeededAsync(ICrudViewModel viewModel)
        {
            AdminPostViewModel categoryViewModel = viewModel as AdminPostViewModel;

            if (categoryViewModel != null)
            {
                var allCategories = await _PostService.GetAllCategoriesAsync();
                var allTags = await _TagService.GetAllTagsAsync();

                categoryViewModel.Categories = GetCategoriesSelectList(allCategories, categoryViewModel.SelectedCategories.Where(o => o.HasValue).Select(o => o.Value).ToArray());

                //categoryViewModel.Tags = GetTagsSelectList(allTags, categoryViewModel.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray());
                var selectedTags = categoryViewModel.SelectedTags != null
                    ? categoryViewModel.SelectedTags.Where(o => o.HasValue).Select(o => o.Value).ToArray()
                    : new int[0];

                categoryViewModel.Tags = GetTagsSelectList(allTags, selectedTags);

                var allImages = await _ImageService.GetAllImagesAsync();
                categoryViewModel.Images = GetImagesSelectList(allImages);
            }
        }

        #endregion

        #region Ovverides

        protected override async Task AfterSaveAndResultDeterminedAsync()
        {
            SetupSitemapService();
            await _SitemapService.TryGenerateAndSaveAsync(HostingEnvironment.MapPath("~/sitemap.xml"));
        }

        private void SetupSitemapService()
        {
            string baseUrl = string.Format("{0}://{1}{2}"
                , Request.Url.Scheme
                , Request.Url.Authority
                , Url.Content("~")
            )
            .TrimEnd('/');

            // This is crap

            _SitemapService.CategoriesBaseUrl = string.Format("{0}/category", baseUrl);
            _SitemapService.PostsBaseUrl = string.Format("{0}/post", baseUrl);
            _SitemapService.EventsBaseUrl = string.Format("{0}/event", baseUrl);
        }

        #endregion

        #region Private Methods

        private void TransferDataToViewModel(Post entity, AdminPostViewModel result)
        {
            result.SelectedCategories = entity.Categories // categories should be ALL for this entity
                .Select(o => o.Id)
                .Cast<int?>()
                .ToArray();

            result.SelectedTags = entity.Tags
                .Select(o => o.Id)
                .Cast<int?>()
                .ToArray();

            result.Content = entity.Content;
            result.Description = entity.Description;
            result.Slug = entity.Slug;
            result.Status = entity.Status;
            result.Title = entity.Title;

            result.Published = entity.Published;

            result.PreviewImageId = entity.PreviewImageId;
            result.DetailImageId = entity.DetailImageId;
        }

        private List<SelectListItem> GetImagesSelectList(IEnumerable<Image> allImages)
        {
            List<SelectListItem> tagSelectList = new List<SelectListItem>();

            foreach (var item in allImages)
            {
                tagSelectList.Add(new SelectListItem
                {
                    Text = string.Format("{0} ({1})", item.Name, item.SmallImage),
                    Value = item.Id.ToString()
                });
            }

            tagSelectList.Insert(0, new SelectListItem { Text = "Please choose", Value = "" });
            return tagSelectList;
        }

        private List<SelectListItem> GetTagsSelectList(IEnumerable<Tag> allTags, int[] selected)
        {
            List<SelectListItem> tagSelectList = new List<SelectListItem>();

            foreach (var item in allTags)
            {
                tagSelectList.Add(new SelectListItem
                {
                    Selected = selected != null && selected.Contains(item.Id),
                    Text = item.Title,
                    Value = item.Id.ToString()
                });
            }

            return tagSelectList;
        }

        private static void TransferFromViewModelToModel(AdminPostViewModel vm, IEnumerable<Category> categories, IEnumerable<Tag> tags, Post post)
        {
            SetCategories(categories, post);
            SetTags(tags, post);

            post.Content = vm.Content;
            post.Description = vm.Description;
            post.Slug = vm.Slug;
            post.Status = vm.Status;
            post.Title = vm.Title;

            post.Published = vm.Published;
        }

        private static void SetTags(IEnumerable<Tag> tags, Post post)
        {
            post.Tags.Clear();
            foreach (var item in tags)
            {
                post.Tags.Add(item);
            }
        }

        private static void SetCategories(IEnumerable<Category> categories, Post post)
        {
            post.Categories.Clear();
            foreach (var item in categories)
            {
                post.Categories.Add(item);
            }
        }

        #endregion
    }
}