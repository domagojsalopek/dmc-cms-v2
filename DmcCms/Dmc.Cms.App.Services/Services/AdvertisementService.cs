using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;

namespace Dmc.Cms.App.Services
{
    public class AdvertisementService : ServiceBase, IAdvertisementService
    {
        public AdvertisementService(ICmsUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<ServiceResult> InsertAsync(Advertisement entity)
        {
            UnitOfWork.AdvertisementRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Advertisement entity)
        {
            UnitOfWork.AdvertisementRepository.Update(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> DeleteAsync(Advertisement entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Advertisement>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.AdvertisementRepository.GetPagedAsync(page, perPage);
        }

        public Task<int> CountAsync()
        {
            return UnitOfWork.AdvertisementRepository.CountAsync();
        }

        public Task<Advertisement> GetByIdAsync(int id)
        {
            return UnitOfWork.AdvertisementRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Advertisement>> GetAllVisibleOfTypeAsync(AdvertisementType advertisementType)
        {
            return await UnitOfWork.AdvertisementRepository.GetAllVisibleOfTypeAsync(advertisementType);
        }

        public async Task<Advertisement> GetVisibleByUniqueIdAsync(Guid uniqueId)
        {
            return await UnitOfWork.AdvertisementRepository.GetVisibleByUniqueIdAsync(uniqueId);
        }

        public async Task<IEnumerable<Advertisement>> GetVisibleForUniqueIdsAsync(IEnumerable<Guid> uniqueIds)
        {
            if (uniqueIds == null)
            {
                throw new ArgumentNullException(nameof(uniqueIds));
            }

            return await UnitOfWork.AdvertisementRepository.GetVisibleForUniqueIdsAsync(uniqueIds);
        }

        public async Task<Advertisement> GetRandomVisibleOfTypeAsync(AdvertisementType advertisementType)
        {
            return await UnitOfWork.AdvertisementRepository.GetRandomVisibleOfTypeAsync(advertisementType);
        }
    }
}
