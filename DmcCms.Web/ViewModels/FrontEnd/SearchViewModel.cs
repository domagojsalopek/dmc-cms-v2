using Dmc.Cms.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.ViewModels
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Results = new PagedList<SearchResultViewModel>(new List<SearchResultViewModel>(), 1, 10, 0);
        }

        public int NumberOfResults
        {
            get;
            set;
        }

        public string[] SearchPhrases
        {
            get;
            set;
        }

        public string SearchQuery
        {
            get;
            set;
        }

        public PagedList<SearchResultViewModel> Results
        {
            get;
            set;
        }
    }
}