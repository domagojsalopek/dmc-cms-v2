using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using Dmc.Cms.Core;
using Dmc.Cms.Model;
using Dmc.Core;
using Dmc.Identity;
using Dmc.Cms.Web.Attributes;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web
{
    // The whole service layer is a BIG HUGE mistake for CRUD.
    // Especially because we don't have dependency injection working as it should.

    // TODO: think. Should we use something for DI or ditch service ... ?
    // Now we don't have much time to play, so let's finish it like this.

    // TODO: fix dependency injection so 

    public abstract class CRUDControllerBase<T> : CRUDControllerBase<T, ICrudViewModel, ICrudViewModel> where T : class, IEntity
    {
        public CRUDControllerBase(ICrudService<T> crudService, ApplicationUserManager manager) 
            : base(crudService, manager)
        {
        }
    }

    public abstract class CRUDControllerBase<T, TViewModel> : CRUDControllerBase<T, TViewModel, TViewModel> 
        where T : class, IEntity
        where TViewModel : class, ICrudViewModel
    {
        public CRUDControllerBase(ICrudService<T> crudService, ApplicationUserManager manager)
            : base(crudService, manager)
        {
        }
    }

    [Authorize(Roles = RoleKeys.Admin)]
    [NoCache]
    public abstract class CRUDControllerBase<T, TCrudViewModel, TBrowseViewModel> : ControllerBase
        where T : class, IEntity
        where TCrudViewModel : class, ICrudViewModel
        where TBrowseViewModel : class, ICrudViewModel
    {
        #region Constants

        // this should be in settings or whereveer, but for start we won't use pagination at all.
        public const int DefaultNumberOfItemsPerPage = 100;

        #endregion

        #region Fields

        private readonly ICrudService<T> _CrudService;
        private bool _SavedSuccessFully = false;

        #endregion

        #region Constuctors 

        protected CRUDControllerBase(ICrudService<T> crudService, ApplicationUserManager manager) : base(manager)
        {
            _CrudService = crudService ?? throw new ArgumentNullException(nameof(crudService));
        }

        #endregion

        #region Properties

        protected static readonly Task CompletedTask = Task.FromResult(0);

        protected ICrudService<T> CrudService => _CrudService;

        #endregion

        #region Web Methods

        [HttpGet]
        public virtual async Task<ActionResult> Index(int? page, int? perPage)
        {
            int itemsPerPage = perPage.GetValueOrDefault() > 0
                ? perPage.GetValueOrDefault()
                : DefaultNumberOfItemsPerPage;

            IEnumerable<T> entityList = await _CrudService.GetPagedAsync(page.GetValueOrDefault(1), itemsPerPage);
            int totalNumberOfEntities = await _CrudService.CountAsync();

            // we intentionally separate this, because browseviewmodel might be different than create/edit
            //IEnumerable<ICrudViewModel> viewModelListResult = await CreateViewModelListAsync(entityList);
            List<TBrowseViewModel> viewModelListResult = new List<TBrowseViewModel>();

            foreach (var entity in entityList)
            {
                TBrowseViewModel viewModelForThisEntity = CreateBrowseViewModel(entity);

                if (viewModelForThisEntity != null)
                {
                    viewModelForThisEntity.Id = entity.Id;
                    viewModelListResult.Add(viewModelForThisEntity);
                }
            }

            return View(new EntityList(viewModelListResult, page.GetValueOrDefault(1), itemsPerPage, totalNumberOfEntities));
        }

        [HttpGet]
        public virtual async Task<ActionResult> Create()
        {
            TCrudViewModel viewModel = await CreateViewModelAsync();
            return EditView(viewModel);
        }

        [HttpGet]
        public virtual async Task<ActionResult> Edit(int id)
        {
            T entity = await _CrudService.GetByIdAsync(id);

            if (entity == null)
            {
                return Error();
            }

            TCrudViewModel viewModel = await CreateViewModelAsync();

            if (viewModel == null)
            {
                return Error();
            }

            await TransferDataFromEntityToViewModelAsync(entity, viewModel);

            viewModel.Id = entity.Id;

            return EditView(viewModel);
        }

        #endregion

        #region Protected Methods

        protected virtual async Task<ActionResult> SaveAsync(TCrudViewModel viewModel)
        {
            if (!ModelState.IsValid) // maybe model is not needed, so let callers get it themselves
            {
                await ValidationFailedFillViewModelIfNeededAsync(viewModel);
                return EditView(viewModel);
            }

            ActionResult result = viewModel.Id.HasValue
                ? await UpdateAsync(viewModel)
                : await InsertAsync(viewModel);

            if (_SavedSuccessFully)
            {
                AfterSaveAndResultDetermined();
                await AfterSaveAndResultDeterminedAsync();
            }

            return result;
        }

        protected virtual Task AfterSaveAndResultDeterminedAsync()
        {
            return CompletedTask;
        }

        protected virtual void AfterSaveAndResultDetermined()
        {
            
        }

        public override ActionResult Error()
        {
            return View("Error");
        }

        #endregion

        #region Abstract Methods

        internal abstract TBrowseViewModel CreateBrowseViewModel(T entity);

        internal abstract Task ValidationFailedFillViewModelIfNeededAsync(TCrudViewModel viewModel);

        internal abstract Task<TCrudViewModel> CreateViewModelAsync();

        internal abstract Task<T> CreateEntityFromViewModelAsync(TCrudViewModel viewModel); //DO WE NEED this?? maybe only fill ... 

        internal abstract Task OperationFailedFillViewModelAsync(TCrudViewModel viewModel, T entity);

        internal abstract Task TransferDataFromViewModelToEntityAsync(TCrudViewModel viewModel, T entity);

        internal abstract Task TransferDataFromEntityToViewModelAsync(T entity, TCrudViewModel viewModel);

        #endregion

        #region Private Methods

        private async Task<ActionResult> InsertAsync(TCrudViewModel viewModel)
        {
            T entity = await CreateEntityFromViewModelAsync(viewModel);

            if (entity == null)
            {
                return Error();
            }

            var result = await _CrudService.InsertAsync(entity);

            if (result == null)
            {
                ModelState.AddModelError("", "An error occured, please try again.");
                await OperationFailedFillViewModelAsync(viewModel, entity);

                return EditView(viewModel);
            }

            if (result.Success && entity.Id != default(int))
            {
                _SavedSuccessFully = true;
                CacheHelper.ResetEntireCache();

                AfterInsert(entity);

                return PerformAfterInsertAndReturnEditView(viewModel, entity);
            }

            AddErrorsToState(result.Errors);
            await OperationFailedFillViewModelAsync(viewModel, entity);
            return EditView(viewModel);
        }

        protected virtual void AfterInsert(T entity)
        {
        }

        protected virtual ActionResult EditView(ICrudViewModel viewModel)
        {
            return View(nameof(Edit), viewModel);
        }

        private async Task<ActionResult> UpdateAsync(TCrudViewModel viewModel)
        {
            T entity = await _CrudService.GetByIdAsync(viewModel.Id.Value);

            if (entity == null)
            {
                return Error();
            }

            await TransferDataFromViewModelToEntityAsync(viewModel, entity);

            var result = await _CrudService.UpdateAsync(entity);

            if (result == null) // this shouldn't happen, but just in case ... 
            {
                ModelState.AddModelError("", "An error occured, please try again.");
                await OperationFailedFillViewModelAsync(viewModel, entity);

                return EditView(viewModel);
            }

            if (result.Success) // here shouldn't be any redirection, we simply create viewmodel again and return
            {
                _SavedSuccessFully = true;

                CacheHelper.ResetEntireCache(); // sigh

                AfterUpdate(entity);

                return await UpdateSuccessAsync(entity);
            }

            // refill ... 
            await OperationFailedFillViewModelAsync(viewModel, entity);

            viewModel.Id = entity.Id; // just in case abstr. fcks up

            AddErrorsToState(result.Errors);
            return EditView(viewModel);
        }

        protected virtual void AfterUpdate(T entity)
        {
            
        }

        private async Task<ActionResult> UpdateSuccessAsync(T entity)
        {
            // get again from db, just repeat the same thing as in redirect
            T newlySavedEntity = await _CrudService.GetByIdAsync(entity.Id);

            if (newlySavedEntity == null) // this shouldn't happen
            {
                return Error();
            }

            TCrudViewModel model = await CreateViewModelAsync();

            if (model == null)
            {
                return Error();
            }

            // transfer
            await TransferDataFromEntityToViewModelAsync(newlySavedEntity, model);

            // after abstract method set id
            model.Id = newlySavedEntity.Id;

            // 
            return PerformAfterUpdateAndReturnEditView(model, newlySavedEntity);
        }

        protected virtual ActionResult PerformAfterInsertAndReturnEditView(TCrudViewModel model, T newlySavedEntity)
        {
            return RedirectToAction(nameof(Edit), new { id = newlySavedEntity.Id });
        }

        protected virtual ActionResult PerformAfterUpdateAndReturnEditView(TCrudViewModel model, T newlySavedEntity)
        {
            return EditView(model);
        }

        private void AddErrorsToState(IEnumerable<string> errors) //TODO: message type ... 
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), error);
            }
        }

        #endregion

        #region Helper Methods

        protected List<SelectListItem> GetCategoriesSelectList(IEnumerable<Category> allCategories, int[] selectedCategories, bool insertEmptyCategory = false) //TODO: cache
        {
            List<SelectListItem> categorySelectList = new List<SelectListItem>();
            FillCategorySelectList(categorySelectList, allCategories, selectedCategories);

            if (insertEmptyCategory)
            {
                categorySelectList.Insert(0, new SelectListItem { Text = "Choose category", Value = String.Empty, Selected = true });
            }

            return categorySelectList;
        }

        protected void FillCategorySelectList(List<SelectListItem> categorySelectList, IEnumerable<Category> allCategories, int[] selectedCategories)
        {
            var rootCategories = allCategories.Where(o => o.ParentId == null);

            foreach (Category rootCategory in rootCategories)
            {
                categorySelectList.Add(new SelectListItem { Text = rootCategory.Title, Value = rootCategory.Id.ToString(), Selected = selectedCategories != null && selectedCategories.Contains(rootCategory.Id) });

                if (rootCategory.HasChildren)
                {
                    AppendChildren(categorySelectList, rootCategory.Children, 1, selectedCategories); // children of root are on level 1
                }
            }
        }

        private void AppendChildren(List<SelectListItem> categorySelectList, IEnumerable<Category> children, int level, int[] selectedCategories)
        {
            foreach (Category category in children)
            {
                categorySelectList.Add(new SelectListItem { Text = string.Format("{0}{1}", GetIndentString(level), category.Title), Value = category.Id.ToString(), Selected = selectedCategories != null && selectedCategories.Contains(category.Id) });

                if (category.HasChildren)
                {
                    AppendChildren(categorySelectList, category.Children, level + 1, selectedCategories);
                }
            }
        }

        private string GetIndentString(int level)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < level; i++)
            {
                sb.Append("»");
            }

            return sb.ToString();
        }

        #endregion
    }
}