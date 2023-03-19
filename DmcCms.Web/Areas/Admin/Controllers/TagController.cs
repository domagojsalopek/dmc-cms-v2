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
    public class TagController : CRUDControllerBase<Tag, AdminTagViewModel>
    {
        #region Fields

        private ITagService _TagService;

        #endregion

        #region Constructors

        public TagController(ITagService crudService, ApplicationUserManager userManager) : base(crudService, userManager)
        {
            _TagService = crudService;
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminTagViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminTagViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Implementation

        internal override AdminTagViewModel CreateBrowseViewModel(Tag entity)
        {
            AdminTagViewModel result = new AdminTagViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override Task<Tag> CreateEntityFromViewModelAsync(AdminTagViewModel viewModel)
        {
            Tag result = new Tag();
            TransferDataFromViewModelToModel(viewModel, result);
            return Task.FromResult(result);
        }

        internal override Task<AdminTagViewModel> CreateViewModelAsync()
        {
            return Task.FromResult(new AdminTagViewModel());
        }

        internal override Task OperationFailedFillViewModelAsync(AdminTagViewModel viewModel, Tag entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Tag entity, AdminTagViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override Task TransferDataFromViewModelToEntityAsync(AdminTagViewModel viewModel, Tag entity)
        {
            TransferDataFromViewModelToModel(viewModel, entity);
            return CompletedTask;
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminTagViewModel viewModel)
        {
            return CompletedTask;
        }

        #endregion

        #region Private Helper Methods

        private void TransferDataFromModelToViewModel(Tag entity, AdminTagViewModel result)
        {
            result.Slug = entity.Slug;
            result.Status = entity.Status;
            result.Title = entity.Title;
        }

        private void TransferDataFromViewModelToModel(AdminTagViewModel viewModel, Tag entity)
        {
            entity.Slug = viewModel.Slug;
            entity.Status = viewModel.Status;
            entity.Title = viewModel.Title;
        }

        #endregion
    }
}