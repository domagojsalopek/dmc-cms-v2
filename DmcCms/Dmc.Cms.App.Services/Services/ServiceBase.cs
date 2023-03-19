using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Services
{
    public abstract class ServiceBase : IDisposable
    {
        #region Fields

        private readonly ICmsUnitOfWork _UnitOfWork;

        #endregion

        #region Constructors

        protected ServiceBase(ICmsUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #endregion

        #region Properties

        protected ICmsUnitOfWork UnitOfWork => _UnitOfWork;

        #endregion

        #region Protected Methods

        protected async Task<ServiceResult> SaveAsync() // UOW should throw custom, we should catch ... 
        {
            try
            {
                await _UnitOfWork.SaveAsync();
                return ServiceResult.Succeeded;
            }
            catch (Exception ex)
            {
                return new ServiceResult(ex.Message); // TODO: Friendly ... 
            }
        }

        #endregion

        #region Private Methods

        protected IEnumerable<Category> CreateHierarchyFromAllCategories(IEnumerable<Category> allCategoriesFromDb)
        {
            List<Category> result = new List<Category>();
            IEnumerable<Category> rootCategories = allCategoriesFromDb.Where(o => o.IsRoot);

            foreach (Category rootCategory in rootCategories)
            {
                rootCategory.Children.Clear();
                FillChildren(rootCategory, allCategoriesFromDb);

                // add filled to result
                result.Add(rootCategory);
            }

            return result;
        }

        private void FillChildren(Category category, IEnumerable<Category> allCategoriesFromDb)
        {
            var children = allCategoriesFromDb.Where(o => o.ParentId.HasValue && o.ParentId == category.Id);

            category.Children.Clear();
            foreach (var item in children)
            {
                FillChildren(item, allCategoriesFromDb);
                category.Children.Add(item);
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_UnitOfWork != null)
                    {
                        _UnitOfWork.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServiceBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
