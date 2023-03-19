using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface ITagRepository : IEntityRepository<Tag>
    {
        Task<IEnumerable<Tag>> GetUsedAsync();

        Task<IEnumerable<Tag>> GetAllAsync();

        Task<IEnumerable<Tag>> GetForIdsAsync(int[] ids);

        Task<Tag> FindBySlugAsync(string slug);
    }
}
