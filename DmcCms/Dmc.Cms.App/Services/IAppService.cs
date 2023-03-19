using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = Dmc.Cms.Model.App;

namespace Dmc.Cms.App
{
    public interface IAppService : ICrudService<Application>
    {
        Task<IEnumerable<Application>> GetAllAppsAsync();
    }
}
