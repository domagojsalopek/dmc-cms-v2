using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class PagedList<T>
    {
        private readonly List<T> _InnerList = new List<T>();

        public PagedList(IEnumerable<T> results, int page, int perPage, int total)
        {
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            _InnerList.AddRange(results);
            PageIndex = page;
            PageSize = perPage;
            TotalCount = total;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        }

        public IEnumerable<T> Entities => _InnerList.AsEnumerable();

        public int PageIndex
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        public int TotalCount
        {
            get;
            private set;
        }

        public int TotalPages
        {
            get;
            private set;
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);
    }
}
