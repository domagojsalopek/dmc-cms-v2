using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ITagService : ICrudService<Tag>
    {
        Task<IEnumerable<Tag>> GetUsedTagsAsync();

        Task<IEnumerable<Tag>> GetTagsForIdsAsync(int[] tagIds);

        Task<IEnumerable<Tag>> GetAllTagsAsync();

        Task<Tag> FindBySlugAsync(string slug);
    } 
}
