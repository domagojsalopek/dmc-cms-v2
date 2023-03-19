using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
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
    public class ContactQueryController : CRUDControllerBase<ContactQuery, AdminContactQueryViewModel>
    {
        #region Constructors

        public ContactQueryController(ApplicationUserManager manager, IContactQueryService contactQueryService)
            : base(contactQueryService, manager)
        {

        }

        #endregion

        #region Override Methods

        internal override AdminContactQueryViewModel CreateBrowseViewModel(ContactQuery entity)
        {
            return new AdminContactQueryViewModel
            {
                Email = entity.Email,
                Id = entity.Id,
                IP = entity.IP,
                Message = entity.Message,
                Name = entity.Name,
                Subject = entity.Subject
            };
        }

        internal override Task<ContactQuery> CreateEntityFromViewModelAsync(AdminContactQueryViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        internal override Task<AdminContactQueryViewModel> CreateViewModelAsync()
        {
            return Task.FromResult(new AdminContactQueryViewModel());
        }

        internal override Task OperationFailedFillViewModelAsync(AdminContactQueryViewModel viewModel, ContactQuery entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(ContactQuery entity, AdminContactQueryViewModel viewModel)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromViewModelToEntityAsync(AdminContactQueryViewModel viewModel, ContactQuery entity)
        {
            return CompletedTask;
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminContactQueryViewModel viewModel)
        {
            return CompletedTask;
        }

        #endregion
    }
}