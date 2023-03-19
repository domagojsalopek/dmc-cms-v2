using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class AdvertisementController : CRUDControllerBase<Advertisement, AdminAdvertisementViewModel>
    {
        #region Private Fields

        private readonly IAdvertisementService _AdvertisementService;

        #endregion

        #region Constructor

        public AdvertisementController(IAdvertisementService crudService, ApplicationUserManager userManager)
            : base(crudService, userManager)
        {
            _AdvertisementService = crudService;
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminAdvertisementViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminAdvertisementViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Override Methods

        internal override AdminAdvertisementViewModel CreateBrowseViewModel(Advertisement entity)
        {
            AdminAdvertisementViewModel result = new AdminAdvertisementViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override async Task<Advertisement> CreateEntityFromViewModelAsync(AdminAdvertisementViewModel viewModel)
        {
            Advertisement result = new Advertisement();
            TransferFromViewModelToModel(viewModel, result);
            return result;
        }

        internal override async Task<AdminAdvertisementViewModel> CreateViewModelAsync()
        {
            return new AdminAdvertisementViewModel();
        }

        internal override Task OperationFailedFillViewModelAsync(AdminAdvertisementViewModel viewModel, Advertisement entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Advertisement entity, AdminAdvertisementViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override async Task TransferDataFromViewModelToEntityAsync(AdminAdvertisementViewModel viewModel, Advertisement entity)
        {
            TransferFromViewModelToModel(viewModel, entity);
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminAdvertisementViewModel viewModel)
        {
            return CompletedTask;
        }

        #endregion

        #region Helper Methods

        private void TransferDataFromModelToViewModel(Advertisement entity, AdminAdvertisementViewModel viewModel)
        {
            viewModel.AdvertisementType = entity.AdvertisementType;
            viewModel.Html = entity.Html;
            viewModel.Name = entity.Name;
            viewModel.UniqueId = entity.UniqueId;
            viewModel.VisibleFrom = entity.VisibleFrom;
            viewModel.VisibleTo = entity.VisibleTo;
            viewModel.IsVisible = entity.IsVisible;
        }

        private void TransferFromViewModelToModel(AdminAdvertisementViewModel viewModel, Advertisement entity)
        {
            entity.AdvertisementType = viewModel.AdvertisementType;
            entity.Html = viewModel.Html;
            entity.Name = viewModel.Name;
            entity.VisibleFrom = viewModel.VisibleFrom;
            entity.VisibleTo = viewModel.VisibleTo;
            entity.IsVisible = viewModel.IsVisible;
        }

        #endregion
    }
}