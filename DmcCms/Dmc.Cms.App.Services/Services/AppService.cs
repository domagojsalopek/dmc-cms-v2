using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Cms.App.Helpers;
using Application = Dmc.Cms.Model.App;
using System.Security.Cryptography;

namespace Dmc.Cms.App.Services
{
    public class AppService : ServiceBase, IAppService
    {
        #region Constants

        private const int ClientSecretLength = 32;

        #endregion

        #region Constructors

        public AppService(ICmsUnitOfWork unitOfWork) 
            : base(unitOfWork)
        {
        }

        #endregion

        #region IAppService

        public Task<int> CountAsync()
        {
            return UnitOfWork.AppRepository.CountAsync();
        }

        public Task<ServiceResult> DeleteAsync(Application entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Application>> GetAllAppsAsync()
        {
            return UnitOfWork.AppRepository.GetAllAsync();
        }

        public Task<Application> GetByIdAsync(int id)
        {
            return UnitOfWork.AppRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<Application>> GetPagedAsync(int page, int perPage)
        {
            return UnitOfWork.AppRepository.GetPagedAsync(page, perPage);
        }

        public Task<ServiceResult> InsertAsync(Application entity)
        {
            if (!ClientIdAndSecretPresent(entity)) // generate things
            {
                entity.ClientId = GenerateClientId();
                entity.ClientSecret = GenerateSecret();
            }

            UnitOfWork.AppRepository.Insert(entity);
            return SaveAsync();
        }

        private bool ClientIdAndSecretPresent(Application entity)
        {
            return !string.IsNullOrWhiteSpace(entity.ClientId) && !string.IsNullOrWhiteSpace(entity.ClientSecret);
        }

        public Task<ServiceResult> UpdateAsync(Application entity)
        {
            UnitOfWork.AppRepository.Update(entity);
            return SaveAsync();
        }

        #endregion

        #region Additional Public Methods

        public string GenerateSecret()
        {
            using (RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[ClientSecretLength];
                cryptoRandomDataGenerator.GetBytes(buffer);

                return BitConverter.ToString(buffer).Replace("-", string.Empty);
            }
        }

        public string GenerateClientId()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        #endregion
    }
}
