using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;

namespace Dmc.Cms.App.Services
{
    public class ImageService : ServiceBase, IImageService
    {
        #region Constructors

        public ImageService(ICmsUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
        }

        #endregion

        #region IImageService

        public Task<int> CountAsync()
        {
            return UnitOfWork.ImageRepository.CountAsync();
        }

        public Task<ServiceResult> DeleteAsync(Image entity)
        {
            throw new NotImplementedException();
        }

        public Task<Image> GetByIdAsync(int id)
        {
            return UnitOfWork.ImageRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Image>> GetAllImagesAsync()
        {
            return UnitOfWork.ImageRepository.GetAllAsync();
        }

        public Task<IEnumerable<Image>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.ImageRepository.GetPagedAsync(page, perPage);
        }

        public Task<ServiceResult> InsertAsync(Image entity)
        {
            UnitOfWork.ImageRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Image entity)
        {
            UnitOfWork.ImageRepository.Update(entity);
            return SaveAsync();
        }

        #endregion
    }
}
