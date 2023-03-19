using System.Collections.Generic;
using Dmc.Cms.Model;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IImageRepository : IEntityRepository<Image>
    {
        Task<IEnumerable<Image>> GetAllAsync();
    }
}