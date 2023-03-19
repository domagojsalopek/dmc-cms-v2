using Dmc.Cms.Model;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal class AdvertisementRepository : EntityRepositoryBase<Advertisement>, IAdvertisementRepository
    {
        private static readonly Random _Random = new Random();

        public AdvertisementRepository(IRepository<Advertisement> repository) : base(repository)
        {
        }

        public Task<IEnumerable<Advertisement>> GetAllAsync()
        {
            return Repository.Query().GetEntitiesAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetAllVisibleOfTypeAsync(AdvertisementType advertisementType)
        {
            IQuery<Advertisement> query = PrepareActiveAdvertisementsQuery();

            return await query
                .Filter(o => o.AdvertisementType == advertisementType)
                .GetEntitiesAsync();
        }

        public async Task<Advertisement> GetVisibleByUniqueIdAsync(Guid uniqueId)
        {
            IQuery<Advertisement> query = PrepareActiveAdvertisementsQuery();

            return await query
                .Filter(o => o.UniqueId == uniqueId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Advertisement>> GetVisibleForUniqueIdsAsync(IEnumerable<Guid> uniqueIds)
        {
            IQuery<Advertisement> query = PrepareActiveAdvertisementsQuery();

            return await query
                .Filter(o => uniqueIds.Contains(o.UniqueId))
                .GetEntitiesAsync();
        }

        public async Task<Advertisement> GetRandomVisibleOfTypeAsync(AdvertisementType advertisementType)
        {
            IQuery<Advertisement> query = PrepareActiveAdvertisementsQuery();
            double rnd = _Random.NextDouble();

            return await query
                .Filter(o => o.AdvertisementType == advertisementType)
                .OrderBy(o => o.OrderBy(f => SqlFunctions.Checksum(f.Id * rnd)))
                .FirstOrDefaultAsync();
        }

        private IQuery<Advertisement> PrepareActiveAdvertisementsQuery()
        {
            var now = DateTimeOffset.UtcNow;

            var query = Repository.Query()
                .Filter(o =>
                    o.IsVisible
                    &&
                    (
                        (!o.VisibleFrom.HasValue)
                        ||
                        (o.VisibleFrom.HasValue && now >= o.VisibleFrom)
                    )
                    &&
                    (
                        (!o.VisibleTo.HasValue)
                        ||
                        (o.VisibleTo.HasValue && o.VisibleTo >= now)
                    )
                );

            return query;
        }
    }
}
