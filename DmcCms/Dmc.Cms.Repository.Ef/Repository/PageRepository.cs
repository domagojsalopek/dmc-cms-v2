using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;

namespace Dmc.Cms.Repository.Ef
{
    internal class PageRepository : ContentRepositoryBase<Page>, IPageRepository
    {
        public PageRepository(IRepository<Page> repository) : base(repository)
        {
        }

        public override async Task<Page> GetByIdAsync(int id)
        {
            return await Repository.Query()
                .Filter(o => o.Id == id)
                .Include(o => o.PreviewImage)
                .Include(o => o.DetailImage)
                .FirstOrDefaultAsync();
        }

        public override async Task<Page> GetBySlugAsync(string slug)
        {
            return await Repository.Query()
                .Filter(o => o.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.PreviewImage)
                .Include(o => o.DetailImage)
                .Include(o => o.Author)
                .FirstOrDefaultAsync();
        }
    }
}
