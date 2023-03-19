using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class Search
    {
        public Search()
        {
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

        public PagedList<SearchResult> Results
        {
            get;
            set;
        }
    }
}
