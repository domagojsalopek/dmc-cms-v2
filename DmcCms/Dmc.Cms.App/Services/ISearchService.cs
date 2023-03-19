using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ISearchService : IService
    {
        Task<Search> SearchPostsAsync(string query, int page, int perPage);
    }
}
