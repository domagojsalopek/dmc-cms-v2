using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class RoleStore<TUser, TRole> : IRoleStore<TRole, int>
        where TUser : class, IUser<int, TRole, IdentityClaim, IdentityLogin>
        where TRole : class, IRole
    {
        private readonly IIdentityUnitOfWork<TUser, int, TRole, IdentityClaim, IdentityLogin> _UnitOFWork;

        public RoleStore(IIdentityUnitOfWork<TUser, int, TRole, IdentityClaim, IdentityLogin> unitOfWork)
        {
            _UnitOFWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IIdentityUnitOfWork<TUser, int, TRole, IdentityClaim, IdentityLogin> UnitOFWork => _UnitOFWork;

        public Task<MembershipResult> CreateAsync(TRole role)
        {
            UnitOFWork.RoleRepository.Insert(role);
            return SaveAsync();
        }

        public Task<MembershipResult> DeleteAsync(TRole role)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByNameAsync(string name)
        {
            return UnitOFWork.RoleRepository
                .Query()
                .Filter(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefaultAsync();
        }

        public Task<MembershipResult> UpdateAsync(TRole role)
        {
            throw new NotImplementedException();
        }

        #region Protected Methods

        protected async Task<MembershipResult> SaveAsync()
        {
            try
            {
                await UnitOFWork.SaveAsync();
                return MembershipResult.Success;
            }
            catch (Exception)
            {
                return MembershipResult.Failed(IdentityCodes.SavingError, "Error Occured"); // TODO: Friendly ... 
            }
        }

        #endregion
    }
}
