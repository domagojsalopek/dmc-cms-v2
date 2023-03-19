using Dmc.Cms.Model;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal class AppRepository : EntityRepositoryBase<App>, IAppRepository
    {
        public AppRepository(IRepository<App> repository) : base(repository)
        {
        }

        public Task<IEnumerable<App>> GetAllAsync()
        {
            return Repository.Query().GetEntitiesAsync();
        }

        public Task<App> GetByClientIdAsync(string clientId)
        {
            return Repository.Query()
                .Filter(o => o.ClientId.Equals(clientId, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
        }
    }
}
