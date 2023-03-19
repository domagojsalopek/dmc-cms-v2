using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IContentRepository<T> : IEntityRepository<T> where T : class, IContent
    {
        Task<T> GetByUniqueIdAsync(Guid uniqueId);

        Task<T> GetBySlugAsync(string slug);

        Task<int> CountPublishedAsync();

        Task<int> CountPublishedAndDraftsAsync();

        Task<IEnumerable<T>> GetPagedPublishedAsync(int page, int perPage);

        Task<IEnumerable<T>> GetPagedPublishedAndDraftsAsync(int page, int perPage);
    }
}
