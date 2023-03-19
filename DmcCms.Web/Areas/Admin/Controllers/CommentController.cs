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
    public class CommentController : CRUDControllerBase<Comment, AdminCommentViewModel>
    {
        public CommentController(ICommentService crudService, ApplicationUserManager manager) : base(crudService, manager)
        {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AdminCommentViewModel viewModel)
        {
            return new HttpUnauthorizedResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AdminCommentViewModel viewModel)
        {
            return await SaveAsync(viewModel);
        }

        internal override AdminCommentViewModel CreateBrowseViewModel(Comment entity)
        {
            var vm = new AdminCommentViewModel();

            TransferFromModelToViewModel(entity, vm);

            return vm;
        }

        private void TransferFromModelToViewModel(Comment entity, AdminCommentViewModel vm)
        {
            vm.Approved = entity.Approved;
            vm.Author = entity.Author;
            vm.Email = entity.Email;
            vm.Id = entity.Id;
            vm.IP = entity.IP;
            vm.Status = entity.Status;
            vm.Text = entity.Text;
        }

        internal override Task<Comment> CreateEntityFromViewModelAsync(AdminCommentViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        internal override Task<AdminCommentViewModel> CreateViewModelAsync()
        {
            return Task.FromResult(new AdminCommentViewModel());
        }

        internal override Task OperationFailedFillViewModelAsync(AdminCommentViewModel viewModel, Comment entity)
        {
            return CompletedTask;
        }

        internal override Task TransferDataFromEntityToViewModelAsync(Comment entity, AdminCommentViewModel viewModel)
        {
            TransferFromModelToViewModel(entity, viewModel);
            return CompletedTask;
        }

        internal override Task TransferDataFromViewModelToEntityAsync(AdminCommentViewModel viewModel, Comment entity)
        {
            entity.Approved = viewModel.Approved;
            return CompletedTask;
        }

        internal override Task ValidationFailedFillViewModelIfNeededAsync(AdminCommentViewModel viewModel)
        {
            return CompletedTask;
        }
    }
}