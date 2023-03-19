using Dmc.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;
using Dmc.Repository.Ef;

namespace Dmc.Cms.Repository.Ef
{
    // basically repositories could also have been in the dmc.cms.repository assembly-
    // only unit work work that uses entity framework needed to be here.

    internal class ContentRepositoryBase<T> : EntityRepositoryBase<T>, IContentRepository<T> where T : class, IContent, IModifiedInfo
    {
        public ContentRepositoryBase(IRepository<T> repository) : base(repository)
        {
        }

        public Task<int> CountPublishedAndDraftsAsync()
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Draft || o.Status == ContentStatus.Published)
                .CountAsync();
        }

        public Task<T> GetByUniqueIdAsync(Guid uniqueId)
        {
            return Repository
                .Query()
                .Filter(o => o.UniqueId == uniqueId)
                .FirstOrDefaultAsync();
        }

        public Task<int> CountPublishedAsync()
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Published)
                .CountAsync();
        }

        public virtual Task<T> GetBySlugAsync(string slug)
        {
            return Repository
                .Query()
                .Filter(o => o.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public virtual Task<IEnumerable<T>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Draft || o.Status == ContentStatus.Published)
                .OrderBy(o => o.OrderByDescending(c => c.Published))
                .GetPagedEntitiesAsync(page, perPage);
        }

        public virtual Task<IEnumerable<T>> GetPagedPublishedAsync(int page, int perPage)
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Published)
                .OrderBy(o => o.OrderByDescending(c => c.Published))
                .GetPagedEntitiesAsync(page, perPage);
        }

        protected IQuery<T> GetPublishedQuery()
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Published)
                .OrderBy(o => o.OrderByDescending(c => c.Published));
        }

        protected IQuery<T> GetDraftsAndPublishedQuery()
        {
            return Repository
                .Query()
                .Filter(o => o.Status == ContentStatus.Published || o.Status == ContentStatus.Draft)
                .OrderBy(o => o.OrderByDescending(c => c.Published));
        }
    }
}
