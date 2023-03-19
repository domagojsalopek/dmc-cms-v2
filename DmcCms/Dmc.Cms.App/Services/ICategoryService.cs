using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ICategoryService : ICrudService<Category>, IContentService<Category>
    {
        Task<IEnumerable<PostCountInfo>> CountPostInCategoriesAsync(int[] categoryIds); 

        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        Task<IEnumerable<Category>> GetCategoriesWithPostsAsync();

        Task<IEnumerable<Category>> GetCategoriesForIdsAsync(int[] categoryIds);
    }
}
