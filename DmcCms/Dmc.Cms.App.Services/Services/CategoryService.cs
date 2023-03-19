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
    public class CategoryService : ServiceBase, ICategoryService
    {
        #region Constructor

        public CategoryService(ICmsUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        #region Methods

        public Task<IEnumerable<Category>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.CategoryRepository.GetPagedAsync(page, perPage);
        }

        public Task<int> CountAsync()
        {
            return UnitOfWork.CategoryRepository.CountAsync();
        }

        public Task<Category> GetByIdAsync(int id)
        {
            return UnitOfWork.CategoryRepository.GetByIdAsync(id);
        }

        public async Task<ServiceResult> InsertAsync(Category entity)
        {
            // we only do this when insert
            entity.Slug = GeneralUtilities.Slugify(entity.Title);

            // 
            UnitOfWork.CategoryRepository.Insert(entity);
            return await SaveAsync();
        }

        public async Task<ServiceResult> UpdateAsync(Category entity)
        {
            UnitOfWork.CategoryRepository.Update(entity);
            return await SaveAsync();
        }

        public Task<ServiceResult> DeleteAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync() // we'll prepare this with a hierarchy.
        {
            var allCategoriesFromDb = await UnitOfWork.CategoryRepository.GetAllCategoriesAsync();
            
            return CreateHierarchyFromAllCategories(allCategoriesFromDb);
        }

        public Task<IEnumerable<Category>> GetCategoriesForIdsAsync(int[] categoryIds)
        {
            return UnitOfWork.CategoryRepository.GetCategoriesForIdsAsync(categoryIds);
        }

        public Task<Category> FindBySlugAsync(string slug)
        {
            return UnitOfWork.CategoryRepository.GetBySlugAsync(slug);
        }

        public Task<IEnumerable<Category>> GetPagedPublishedAndDraftsAsync(int page, int perPage)
        {
            return UnitOfWork.CategoryRepository.GetPagedPublishedAndDraftsAsync(page, perPage);
        }

        public Task<IEnumerable<Category>> GetPagedPublishedAsync(int page, int perPage)
        {
            return UnitOfWork.CategoryRepository.GetPagedPublishedAsync(page, perPage);
        }

        public Task<int> CountAllPublishedAndDraftsAsync()
        {
            return UnitOfWork.CategoryRepository.CountPublishedAndDraftsAsync();
        }

        public Task<int> CountAllPublishedAsync()
        {
            return UnitOfWork.CategoryRepository.CountPublishedAsync();
        }

        public Task<Category> FindByUniqueIdAsync(Guid uniqueId)
        {
            return UnitOfWork.CategoryRepository.GetByUniqueIdAsync(uniqueId);
        }

        public Task<IEnumerable<Category>> GetCategoriesWithPostsAsync()
        {
            return UnitOfWork.CategoryRepository.GetCategoriesWithPostsAsync();
        }

        public Task<IEnumerable<PostCountInfo>> CountPostInCategoriesAsync(int[] categoryIds)
        {
            return UnitOfWork.CategoryRepository.CountPostInCategoriesAsync(categoryIds);
        }

        #endregion
    }
}
