using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;

namespace Dmc.Cms.App.Services
{
    public class ContactService : ServiceBase, IContactQueryService
    {
        public ContactService(ICmsUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<int> CountAsync()
        {
            return UnitOfWork.ContactRepository.CountAsync();
        }

        public Task<ServiceResult> DeleteAsync(ContactQuery entity)
        {
            UnitOfWork.ContactRepository.Delete(entity);
            return SaveAsync();
        }

        public Task<ContactQuery> GetByIdAsync(int id)
        {
            return UnitOfWork.ContactRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<ContactQuery>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.ContactRepository.GetPagedAsync(page, perPage);
        }

        public Task<ServiceResult> InsertAsync(ContactQuery entity)
        {
            UnitOfWork.ContactRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(ContactQuery entity)
        {
            UnitOfWork.ContactRepository.Update(entity);
            return SaveAsync();
        }
    }
}
