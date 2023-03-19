using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IEventService : ICrudService<Event>, IContentService<Event>
    {
        Task<IEnumerable<Event>> GetRecurringEventsAsync(DateTime dateTime);
    }
}
