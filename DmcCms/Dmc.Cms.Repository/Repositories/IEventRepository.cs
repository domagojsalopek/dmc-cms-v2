﻿using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository
{
    public interface IEventRepository : IContentRepository<Event>
    {
        Task<IEnumerable<Event>> GetRecurringAsync(DateTime dateTime);
    }
}
