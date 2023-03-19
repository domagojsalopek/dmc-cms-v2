using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App.Services;
using System.Threading.Tasks;
using Dmc.Cms.Web.Mappers;
using Dmc.Cms.App;

namespace Dmc.Cms.Web.Controllers
{
    public class CategoryController : FrontEndControllerBase<Post, PostViewModel>
    {
        //TODO -> Move posts by category from default to here!!!

        #region Private Fields

        private Category _Category;
        private readonly IPostService _PostService;
        private readonly ICategoryService _CategoryService;

        #endregion

        #region Constructors

        public CategoryController(IPostService service, ICategoryService categoryService, ApplicationUserManager manager) : base(service, manager)
        {
            _PostService = service ?? throw new ArgumentNullException(nameof(service));
            _CategoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        #endregion

        #region Web Methods

        public async Task<ActionResult> Index(string categoryName, int? page, int? perPage)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return NotFound();
            }

            _Category = await _CategoryService.FindBySlugAsync(categoryName);

            if (_Category == null || !IsEntityDisplayable(_Category))
            {
                return NotFound();
            }

            EntityList<PostViewModel> entityList = await GetEntityListAsync(page, perPage);
            CategoryViewModel viewModel = CreateCategoryViewModel(_Category, entityList); 

            return View(viewModel);
        }

        private CategoryViewModel CreateCategoryViewModel(Category category, EntityList<PostViewModel> entityList)
        {
            CategoryViewModel result = new CategoryViewModel
            {
                Posts = entityList
            };

            result.Description = category.Description;
            result.Title = category.Title;
            result.Slug = category.Slug;
            result.Status = category.Status;
            result.Id = category.Id;
            
            if (_Category.IntroImage != null)
            {
                result.IntroImage = new ImageViewModel();
                ImageMapper.TransferToViewModel(_Category.IntroImage, result.IntroImage);
            }

            return result;
        }

        #endregion

        #region Abstract Implementation

        internal override void TransferFromEntityToBrowseViewModel(Post entity, PostViewModel viewModel)
        {
            PostMapper.TransferToViewModel(entity, viewModel);
        }

        internal override void TransferFromEntityToDetailsViewModel(Post entity, PostViewModel viewModel)
        {
            throw new NotImplementedException(); // not used here
        }

        #endregion
    }
}