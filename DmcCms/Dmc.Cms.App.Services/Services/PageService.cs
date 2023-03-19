using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.App.Helpers;

namespace Dmc.Cms.App.Services
{
    public class PageService : ServiceBase, IPageService
    {
        #region Constructors

        public PageService(ICmsUnitOfWork unitOfWork) 
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

        public Task<ServiceResult> DeleteAsync(Page entity)
        {
            throw new NotImplementedException();
        }

        public Task<Page> FindBySlugAsync(string slug)
        {
            return UnitOfWork.PageRepository.GetBySlugAsync(slug);
        }

        public Task<Page> GetByIdAsync(int id)
        {
            return UnitOfWork.PageRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Page>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.PageRepository.GetPagedAsync(page, perPage);
        }

        public Task<IEnumerable<Page>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            return UnitOfWork.PageRepository.GetPagedPublishedAndDraftsAsync(page, perPage);
        }

        public Task<IEnumerable<Page>> GetPagedPublishedAsync(int page, int perPage)
        {
            return UnitOfWork.PageRepository.GetPagedPublishedAsync(page, perPage);
        }

        public async Task<ServiceResult> InsertAsync(Page entity)
        {
            // we only do this when insert
            entity.Slug = GeneralUtilities.Slugify(entity.Title);

            // 
            UnitOfWork.PageRepository.Insert(entity);
            return await SaveAsync();
        }

        public async Task<ServiceResult> UpdateAsync(Page entity)
        {
            UnitOfWork.PageRepository.Update(entity);
            return await SaveAsync();
        }

        public Task<Page> FindByUniqueIdAsync(Guid uniqueId)
        {
            return UnitOfWork.PageRepository.GetByUniqueIdAsync(uniqueId);
        }

        #endregion
    }
}
