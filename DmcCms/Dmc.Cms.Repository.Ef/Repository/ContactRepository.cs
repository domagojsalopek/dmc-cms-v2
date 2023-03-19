using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;
using System.Data.Entity;

namespace Dmc.Cms.Repository.Ef
{
    internal class ContactRepository : EntityRepositoryBase<ContactQuery>, IContactRepository
    {
        internal ContactRepository(IRepository<ContactQuery> repository) : base(repository)
        {

        }

        public async Task<IEnumerable<ContactQuery>> GetAllForUserAsync(int userId)
        {
            return await Repository
                .Query()
                .Filter(o => o.UserId == userId)
                .GetEntitiesAsync();
        }
    }
}
