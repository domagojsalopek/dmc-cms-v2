using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dmc.Cms.App.Identity;
using Dmc.Cms.App;
using Dmc.Cms.App.Services;
using Dmc.Cms.Web.ViewModels;

namespace Dmc.Cms.Web.Controllers
{
    public class SearchController : ControllerBase
    {
        #region Fields

        private readonly ISearchService _SearchService;

        #endregion

        #region Constructors

        public SearchController(ISearchService service, ApplicationUserManager manager) 
            : base(manager)
        {
            _SearchService = service ?? throw new ArgumentNullException(nameof(service));
        }

        #endregion

        public async Task<ActionResult> Index(string q, int? page)
        {
            if (string.IsNullOrWhiteSpace(q)) // this should be prevented by validation.
            {
                return RedirectToLocal("/");
            }

            page = (!page.HasValue || (page.HasValue && page.Value < 1)) ? 1 : page;
            Search results = await _SearchService.SearchPostsAsync(q, page.Value, AppConfig.SiteSettings.PostsPerPage);

            if (results.NumberOfResults <= 0)
            {
                return View("NothingFound", (object)q);
            }

            SearchViewModel viewModel = CreateFromSearch(results);


            return View(viewModel);
        }

        private SearchViewModel CreateFromSearch(Search results)
        {
            SearchViewModel viewModel = new SearchViewModel();
            if (results == null)
            {
                return viewModel;
            }

            viewModel.NumberOfResults = results.NumberOfResults;
            viewModel.SearchPhrases = results.SearchPhrases;
            viewModel.SearchQuery = results.SearchQuery;
            viewModel.Results = CreateResults(results.Results);

            return viewModel;
        }

        private PagedList<SearchResultViewModel> CreateResults(PagedList<SearchResult> results)
        {
            List<SearchResultViewModel> innerList = CreateViewModelList(results.Entities);
            return new PagedList<SearchResultViewModel>(innerList, results.PageIndex, results.PageSize, results.TotalCount);
        }

        private List<SearchResultViewModel> CreateViewModelList(IEnumerable<SearchResult> entities)
        {
            if (entities == null)
            {
                return new List<SearchResultViewModel>();
            }

            List<SearchResultViewModel> result = new List<SearchResultViewModel>();

            foreach (var item in entities)
            {
                result.Add(CreateViewModel(item));
            }

            return result;
        }

        private SearchResultViewModel CreateViewModel(SearchResult item)
        {
            SearchResultViewModel result = new SearchResultViewModel
            {
                Excerpt = item.DescriptionExcerpt,
                Slug = item.Slug,
                Title = item.Title,
                FullTextExcerpt = item.ContentExcerpt
            };

            if (item.PreviewImage != null)
            {
                result.PreviewImage = new ImageViewModel
                {
                    AltText = item.PreviewImage.AltText,
                    Description = item.PreviewImage.Caption,
                    LargeImage = item.PreviewImage.LargeImage,
                    Name = item.PreviewImage.Name,
                    SmallImage = item.PreviewImage.SmallImage
                };
            }

            return result;
        }
    }
}