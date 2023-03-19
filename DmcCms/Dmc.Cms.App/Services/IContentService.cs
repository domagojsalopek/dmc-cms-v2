using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IContentService<T> : IService<T> where T : class, IContent
    {
        Task<IEnumerable<T>> GetPagedPublishedAndDraftsAsync(int page, int perPage);

        Task<IEnumerable<T>> GetPagedPublishedAsync(int page, int perPage);

        Task<int> CountAllPublishedAndDraftsAsync();

        Task<int> CountAllPublishedAsync();

        Task<T> FindBySlugAsync(string slug);

        Task<T> FindByUniqueIdAsync(Guid uniqueId);
    }
}
