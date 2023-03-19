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
using Application = Dmc.Cms.Model.App;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class AppController : CRUDControllerBase<Application, AdminAppViewModel>
    {
        #region Fields

        private IAppService _AppService;

        #endregion

        #region Constructors

        public AppController(IAppService crudService, ApplicationUserManager userManager) 
            : base(crudService, userManager)
        {
            _AppService = crudService;
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminAppViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminAppViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Implementation

        internal override AdminAppViewModel CreateBrowseViewModel(Application entity)
        {
            AdminAppViewModel result = new AdminAppViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override Task<Application> CreateEntityFromViewModelAsync(AdminAppViewModel viewModel)
        {
            Application result = new Application();
            TransferDataFromViewModelToModel(viewModel, result);
            return Task.FromResult(result);
        }

        internal override Task<AdminAppViewModel> CreateViewModelAsync()
        {
            return Task.FromResult(new AdminAppViewModel());
        }

        internal override Task OperationFailedFillViewModelAsync(AdminAppViewModel viewModel, Application entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Application entity, AdminAppViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override Task TransferDataFromViewModelToEntityAsync(AdminAppViewModel viewModel, Application entity)
        {
            TransferDataFromViewModelToModel(viewModel, entity);
            return CompletedTask;
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminAppViewModel viewModel)
        {
            return CompletedTask;
        }

        #endregion

        #region Private Helper Methods

        private void TransferDataFromModelToViewModel(Application entity, AdminAppViewModel result)
        {
            result.Name = entity.Name;
            result.Description = entity.Description;
            result.ClientId = entity.ClientId;
            result.ClientSecret = entity.ClientSecret;
        }

        private void TransferDataFromViewModelToModel(AdminAppViewModel viewModel, Application entity)
        {
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
        }

        #endregion
    }
}