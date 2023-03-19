using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Cms.App.Helpers;

namespace Dmc.Cms.App.Services
{
    public class TagService : ServiceBase, ITagService
    {
        #region Constructors

        public TagService(ICmsUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
        }

        #endregion

        #region ITagService

        public Task<int> CountAsync()
        {
            return UnitOfWork.TagRepository.CountAsync();
        }

        public Task<ServiceResult> DeleteAsync(Tag entity)
        {
            throw new NotImplementedException();
        }

        public Task<Tag> GetByIdAsync(int id)
        {
            return UnitOfWork.TagRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Tag>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.TagRepository.GetPagedAsync(page, perPage);
        }

        public Task<IEnumerable<Tag>> GetUsedTagsAsync()
        {
            return UnitOfWork.TagRepository.GetUsedAsync();
        }

        public Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return UnitOfWork.TagRepository.GetAllAsync();
        }

        public Task<IEnumerable<Tag>> GetTagsForIdsAsync(int[] tagIds)
        {
            return UnitOfWork.TagRepository.GetForIdsAsync(tagIds);
        }

        public Task<ServiceResult> InsertAsync(Tag entity)
        {
            entity.Slug = GeneralUtilities.Slugify(entity.Title);
            UnitOfWork.TagRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Tag entity)
        {
            UnitOfWork.TagRepository.Update(entity);
            return SaveAsync();
        }

        public Task<Tag> FindBySlugAsync(string slug)
        {
            return UnitOfWork.TagRepository.FindBySlugAsync(slug);
        }

        #endregion
    }
}
