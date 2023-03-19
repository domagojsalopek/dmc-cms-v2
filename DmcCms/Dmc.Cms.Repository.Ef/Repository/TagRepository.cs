using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;

namespace Dmc.Cms.Repository.Ef
{
    internal class TagRepository : EntityRepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IRepository<Tag> repository) : base(repository)
        {
        }

        public Task<Tag> FindBySlugAsync(string slug)
        {
            return Repository.Query()
                .Filter(o => o.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public Task<IEnumerable<Tag>> GetAllAsync()
        {
            return Repository.Query().GetEntitiesAsync();
        }

        public Task<IEnumerable<Tag>> GetUsedAsync()
        {
            return Repository.Query()
                .Filter(o => o.Posts.Any(p => p.Status == Core.ContentStatus.Published))
                .GetEntitiesAsync();
        }

        public Task<IEnumerable<Tag>> GetForIdsAsync(int[] ids)
        {
            return Repository
                .Query()
                .Filter(o => ids.Contains(o.Id))
                .GetEntitiesAsync();
        }
    }
}
