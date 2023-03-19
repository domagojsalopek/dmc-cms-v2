using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Services;
using System.Threading.Tasks;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class PageController : CRUDControllerBase<Page, AdminPageViewModel>
    {
        #region Private Fields

        private readonly IPageService _PageService;
        private readonly IImageService _ImageService;

        #endregion

        #region Constructor

        public PageController(IPageService crudService, IImageService imageService, ApplicationUserManager userManager) 
            : base(crudService, userManager)
        {
            _PageService = crudService;
            _ImageService = imageService;
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminPageViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminPageViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Override Methods

        internal override AdminPageViewModel CreateBrowseViewModel(Page entity)
        {
            AdminPageViewModel result = new AdminPageViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override async Task<Page> CreateEntityFromViewModelAsync(AdminPageViewModel viewModel)
        {
            Page result = new Page();
            TransferFromViewModelToModel(viewModel, result);
            result.PreviewImage = viewModel.PreviewImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.PreviewImageId.Value) : null;
            result.DetailImage = viewModel.DetailImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.DetailImageId.Value) : null;
            return result;
        }

        internal override async Task<AdminPageViewModel> CreateViewModelAsync()
        {
            var vm = new AdminPageViewModel();
            vm.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
            return vm;
        }

        internal override async Task OperationFailedFillViewModelAsync(AdminPageViewModel viewModel, Page entity)
        {
            viewModel.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Page entity, AdminPageViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override async Task TransferDataFromViewModelToEntityAsync(AdminPageViewModel viewModel, Page entity)
        {
            entity.PreviewImage = viewModel.PreviewImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.PreviewImageId.Value) : null;
            entity.DetailImage = viewModel.DetailImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.DetailImageId.Value) : null;
            TransferFromViewModelToModel(viewModel, entity);
        }

        internal override async Task ValidationFailedFillViewModelIfNeededAsync(AdminPageViewModel viewModel)
        {
            viewModel.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
        }

        #endregion

        #region Private Helper Methods

        private void TransferFromViewModelToModel(AdminPageViewModel viewModel, Page result)
        {
            result.Content = viewModel.Content;
            result.Description = viewModel.Description;
            result.Order = viewModel.Order.GetValueOrDefault();
            result.Slug = viewModel.Slug;
            result.Status = viewModel.Status;
            result.Title = viewModel.Title;
            result.Published = viewModel.Published;
        }

        private void TransferDataFromModelToViewModel(Page entity, AdminPageViewModel viewModel)
        {
            viewModel.Content = entity.Content;
            viewModel.Description = entity.Description;
            viewModel.Order = entity.Order;
            viewModel.Slug = entity.Slug;
            viewModel.Status = entity.Status;
            viewModel.Title = entity.Title;
            viewModel.Published = entity.Published;

            viewModel.PreviewImageId = entity.PreviewImageId;
            viewModel.DetailImageId = entity.DetailImageId;
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