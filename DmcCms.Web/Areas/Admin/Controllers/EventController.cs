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
    public class EventController : CRUDControllerBase<Event, AdminEventViewModel>
    {
        #region Private Fields

        private readonly IEventService _EventService;
        private readonly IImageService _ImageService;

        #endregion

        #region Constructor

        public EventController(IEventService crudService, IImageService imageService, ApplicationUserManager userManager)
            : base(crudService, userManager)
        {
            _EventService = crudService;
            _ImageService = imageService;
        }

        #endregion

        #region Web Methods

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminEventViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminEventViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        #endregion

        #region Abstract Override Methods

        internal override AdminEventViewModel CreateBrowseViewModel(Event entity)
        {
            AdminEventViewModel result = new AdminEventViewModel();
            TransferDataFromModelToViewModel(entity, result);
            return result;
        }

        internal override async Task<Event> CreateEntityFromViewModelAsync(AdminEventViewModel viewModel)
        {
            Event result = new Event();
            TransferFromViewModelToModel(viewModel, result);
            result.Image = viewModel.ImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.ImageId.Value) : null;
            return result;
        }

        internal override async Task<AdminEventViewModel> CreateViewModelAsync()
        {
            var vm = new AdminEventViewModel();
            vm.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
            return vm;
        }

        internal override async Task OperationFailedFillViewModelAsync(AdminEventViewModel viewModel, Event entity)
        {
            viewModel.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Event entity, AdminEventViewModel viewModel)
        {
            TransferDataFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override async Task TransferDataFromViewModelToEntityAsync(AdminEventViewModel viewModel, Event entity)
        {
            entity.Image = viewModel.ImageId.HasValue ? await _ImageService.GetByIdAsync(viewModel.ImageId.Value) : null;
            TransferFromViewModelToModel(viewModel, entity);
        }

        internal override async Task ValidationFailedFillViewModelIfNeededAsync(AdminEventViewModel viewModel)
        {
            viewModel.Images = GetImagesSelectList(await _ImageService.GetAllImagesAsync());
        }

        #endregion

        #region Private Helper Methods

        private void TransferFromViewModelToModel(AdminEventViewModel viewModel, Event result)
        {
            result.Content = viewModel.Content;
            result.Description = viewModel.Description;
            result.Order = viewModel.Order.GetValueOrDefault();
            result.Slug = viewModel.Slug;
            result.Status = viewModel.Status;
            result.Title = viewModel.Title;
            result.Published = viewModel.Published;
            result.EventType = viewModel.EventType;
            result.EventDate = viewModel.EventDate;
        }

        private void TransferDataFromModelToViewModel(Event entity, AdminEventViewModel viewModel)
        {
            viewModel.Content = entity.Content;
            viewModel.Description = entity.Description;
            viewModel.Order = entity.Order;
            viewModel.Slug = entity.Slug;
            viewModel.Status = entity.Status;
            viewModel.Title = entity.Title;
            viewModel.Published = entity.Published;
            viewModel.ImageId = entity.ImageId;
            viewModel.EventDate = entity.EventDate;
            viewModel.EventType = entity.EventType;
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