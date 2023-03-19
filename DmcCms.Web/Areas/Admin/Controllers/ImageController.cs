using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Services;
using System.Threading.Tasks;
using System.IO;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    public class ImageController : CRUDControllerBase<Image, AdminImageViewModel>
    {
        #region Fields

        private const string UploadDirectory = "~/Uploads/Images";

        #endregion

        #region Constructors

        public ImageController(IImageService crudService, ApplicationUserManager manager) : base(crudService, manager)
        {
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminImageViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminImageViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Implementation

        internal override AdminImageViewModel CreateBrowseViewModel(Image entity)
        {
            AdminImageViewModel result = new AdminImageViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override Task<Image> CreateEntityFromViewModelAsync(AdminImageViewModel viewModel)
        {
            Image result = new Image();
            TransferDataFromViewModelToModel(viewModel, result);
            return Task.FromResult(result);
        }

        internal override Task<AdminImageViewModel> CreateViewModelAsync()
        {
            return Task.FromResult(new AdminImageViewModel());
        }

        internal override Task OperationFailedFillViewModelAsync(AdminImageViewModel viewModel, Image entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Image entity, AdminImageViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override Task TransferDataFromViewModelToEntityAsync(AdminImageViewModel viewModel, Image entity)
        {
            TransferDataFromViewModelToModel(viewModel, entity);
            return CompletedTask;
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminImageViewModel viewModel)
        {
            return CompletedTask;
        }

        #endregion

        protected override void AfterSaveAndResultDetermined()
        {
            ImageInfoProvider.ClearCache();
            base.AfterSaveAndResultDetermined();
        }

        #region Private Helper Methods

        private void TransferDataFromModelToViewModel(Image entity, AdminImageViewModel result)
        {
            result.AltText = entity.AltText;
            result.Caption = entity.Caption;
            result.LargeImageUploadPath = entity.LargeImage;
            result.Name = entity.Name;
            result.SmallImageUploadPath = entity.SmallImage;
        }

        private void TransferDataFromViewModelToModel(AdminImageViewModel viewModel, Image result)
        {
            SetImagePathsToModel(viewModel, result);
            result.AltText = viewModel.AltText;
            result.Caption = viewModel.Caption;
            result.Name = viewModel.Name;
        }

        // we only need to do this if we are changing them. otherwise they just stay the same.
        private void SetImagePathsToModel(AdminImageViewModel viewModel, Image result)
        {
            string rootPath = Server.MapPath(UploadDirectory);
            DirectoryInfo uploadsFolder = new DirectoryInfo(rootPath);

            if (!uploadsFolder.Exists)
            {
                uploadsFolder.Create();
            }

            if (viewModel.SmallImageUpload != null && viewModel.SmallImageUpload.ContentLength > 0)
            {
                viewModel.SmallImageUpload.SaveAs(Path.Combine(uploadsFolder.FullName, viewModel.SmallImageUpload.FileName).ToLower());
                result.SmallImage = "~" + Url.Content(Path.Combine(UploadDirectory, viewModel.SmallImageUpload.FileName).ToLower());
            }

            if (viewModel.LargeImageUpload != null && viewModel.LargeImageUpload.ContentLength > 0)
            {
                viewModel.LargeImageUpload.SaveAs(Path.Combine(uploadsFolder.FullName, viewModel.LargeImageUpload.FileName).ToLower());
                result.LargeImage = "~" + Url.Content(Path.Combine(UploadDirectory, viewModel.LargeImageUpload.FileName).ToLower());
            }
        }

        #endregion
    }
}