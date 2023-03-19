using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System.Text;
using Dmc.Cms.App;
using Dmc.Identity;
using Dmc.Cms.App.Identity;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class CategoryController : CRUDControllerBase<Category>
    {
        #region Private Fields

        private readonly ICategoryService _CategoryService;
        private readonly IImageService _ImageService;

        #endregion

        #region Constructors

        public CategoryController(ICategoryService service, IImageService imageService, ApplicationUserManager manager) 
            : base(service, manager)
        {
            _CategoryService = service ?? throw new ArgumentNullException(nameof(service));
            _ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminCategoryViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminCategoryViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Implementation

        internal override ICrudViewModel CreateBrowseViewModel(Category entity)
        {
            AdminCategoryViewModel viewModel = new AdminCategoryViewModel();
            TransferDataToViewModel(entity, viewModel);
            return viewModel;
        }

        internal override async Task ValidationFailedFillViewModelIfNeededAsync(ICrudViewModel viewModel)
        {
            AdminCategoryViewModel categoryViewModel = viewModel as AdminCategoryViewModel;

            if (categoryViewModel != null) // it will be an error in base .. 
            {
                var categories = await _CategoryService.GetAllCategoriesAsync();
                categoryViewModel.Categories = GetCategoriesSelectList(categories, null, true);

                var images = await _ImageService.GetAllImagesAsync();
                categoryViewModel.Images = GetImagesSelectList(images);
            }
        }

        internal async override Task<Category> CreateEntityFromViewModelAsync(ICrudViewModel viewModel)
        {
            AdminCategoryViewModel categoryViewModel = viewModel as AdminCategoryViewModel;
            Category category = new Category();

            category.Slug = categoryViewModel.Slug;
            category.Status = categoryViewModel.Status;
            category.Title = categoryViewModel.Title;
            category.ParentId = categoryViewModel.ParentId;
            category.Description = categoryViewModel.Description;
            category.Published = categoryViewModel.Published;

            category.IntroImage = categoryViewModel.IntroImageId.HasValue 
                ? await _ImageService.GetByIdAsync(categoryViewModel.IntroImageId.Value) 
                : null;

            return category;
        }

        internal override async Task<ICrudViewModel> CreateViewModelAsync()
        {
            AdminCategoryViewModel result = new AdminCategoryViewModel();
            var categories = await _CategoryService.GetAllCategoriesAsync();
            result.Categories = GetCategoriesSelectList(categories, null, true);

            var images = await _ImageService.GetAllImagesAsync();
            result.Images = GetImagesSelectList(images);
            return result;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Category entity, ICrudViewModel viewModel)
        {
            AdminCategoryViewModel result = viewModel as AdminCategoryViewModel;
            TransferDataToViewModel(entity, result);
            return CompletedTask;
        }

        private static void TransferDataToViewModel(Category entity, AdminCategoryViewModel result)
        {
            result.Description = entity.Description;
            result.ParentId = entity.ParentId;
            result.Slug = entity.Slug;
            result.Status = entity.Status;
            result.Title = entity.Title;
            result.Published = entity.Published;

            result.IntroImageId = entity.IntroImageId;
        }

        internal override async Task OperationFailedFillViewModelAsync(ICrudViewModel viewModel, Category entity)
        {
            AdminCategoryViewModel model = viewModel as AdminCategoryViewModel;

            if (model == null)
            {
                return;
            }

            var categories = await _CategoryService.GetAllCategoriesAsync();
            model.Categories = GetCategoriesSelectList(categories, null, true);

            var images = await _ImageService.GetAllImagesAsync();
            model.Images = GetImagesSelectList(images);
        }

        internal override async Task TransferDataFromViewModelToEntityAsync(ICrudViewModel viewModel, Category entity)
        {
            AdminCategoryViewModel model = viewModel as AdminCategoryViewModel;

            if (model == null)
            {
                return;
            }

            entity.Slug = model.Slug;
            entity.Description = model.Description;
            entity.ParentId = model.ParentId;
            entity.Status = model.Status;
            entity.Title = model.Title;

            entity.IntroImage = model.IntroImageId.HasValue 
                ? await _ImageService.GetByIdAsync(model.IntroImageId.Value) 
                : null;
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}