using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IAdvertisementRepository : IEntityRepository<Advertisement>
    {
        Task<IEnumerable<Advertisement>> GetAllVisibleOfTypeAsync(AdvertisementType advertisementType);

        Task<Advertisement> GetVisibleByUniqueIdAsync(Guid uniqueId);

        Task<IEnumerable<Advertisement>> GetVisibleForUniqueIdsAsync(IEnumerable<Guid> uniqueIds);

        Task<Advertisement> GetRandomVisibleOfTypeAsync(AdvertisementType advertisementType);
    }
}
