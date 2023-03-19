using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Repository;

namespace Dmc.Cms.Repository.Ef
{
    internal class EventRepository : ContentRepositoryBase<Event>, IEventRepository
    {
        public EventRepository(IRepository<Event> repository) : base(repository)
        {
        }

        public override async Task<Event> GetByIdAsync(int id)
        {
            return await Repository.Query()
                .Filter(o => o.Id == id)
                .Include(o => o.Image)
                .FirstOrDefaultAsync();
        }

        public override async Task<Event> GetBySlugAsync(string slug)
        {
            return await Repository.Query()
                .Filter(o => o.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Image)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Event>> GetRecurringAsync(DateTime dateTime)
        {
            int day = dateTime.Day;
            int month = dateTime.Month;

            // recurring means year is ignored.

            return await Repository.Query()
                .Filter(o =>
                    o.Status == Core.ContentStatus.Published
                    &&
                    o.EventDate.Day == day
                    &&
                    o.EventDate.Month == month
                )
                .Include(o => o.Image)
                .GetEntitiesAsync();
        }
    }
}
