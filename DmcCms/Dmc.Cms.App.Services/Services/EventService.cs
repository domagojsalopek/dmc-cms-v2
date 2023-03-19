using Dmc.Cms.App.Helpers;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Services
{
    public class EventService : ServiceBase, IEventService
    {
        #region Constructors

        public EventService(ICmsUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        #endregion

        #region IPageService

        public Task<int> CountAsync()
        {
            return UnitOfWork.PageRepository.CountAsync();
        }

        public Task<int> CountAllPublishedAndDraftsAsync()
        {
            return UnitOfWork.PageRepository.CountPublishedAndDraftsAsync();
        }

        public Task<int> CountAllPublishedAsync()
        {
            return UnitOfWork.PageRepository.CountPublishedAsync();
        }

        public Task<ServiceResult> DeleteAsync(Event entity)
        {
            throw new NotImplementedException();
        }

        public Task<Event> FindBySlugAsync(string slug)
        {
            return UnitOfWork.EventRepository.GetBySlugAsync(slug);
        }

        public Task<Event> GetByIdAsync(int id)
        {
            return UnitOfWork.EventRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Event>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.EventRepository.GetPagedAsync(page, perPage);
        }

        public Task<IEnumerable<Event>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            return UnitOfWork.EventRepository.GetPagedPublishedAndDraftsAsync(page, perPage);
        }

        public Task<IEnumerable<Event>> GetPagedPublishedAsync(int page, int perPage)
        {
            return UnitOfWork.EventRepository.GetPagedPublishedAsync(page, perPage);
        }

        public async Task<ServiceResult> InsertAsync(Event entity)
        {
            // we only do this when insert
            entity.Slug = GeneralUtilities.Slugify(entity.Title);

            // 
            UnitOfWork.EventRepository.Insert(entity);
            return await SaveAsync();
        }

        public async Task<ServiceResult> UpdateAsync(Event entity)
        {
            UnitOfWork.EventRepository.Update(entity);
            return await SaveAsync();
        }

        public Task<Event> FindByUniqueIdAsync(Guid uniqueId)
        {
            return UnitOfWork.EventRepository.GetByUniqueIdAsync(uniqueId);
        }

        public Task<IEnumerable<Event>> GetRecurringEventsAsync(DateTime dateTime)
        {
            return UnitOfWork.EventRepository.GetRecurringAsync(dateTime);
        }

        #endregion
    }
}
