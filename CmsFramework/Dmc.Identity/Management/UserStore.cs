using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class UserStore<TUser> : UserStore<TUser, int, IdentityRole, IdentityClaim, IdentityLogin>, IUserStore<TUser>
        where TUser : class, IUser
    {
        public UserStore(IIdentityUnitOfWork<TUser> unitOfWork) : base(unitOfWork)
        {
        }
    }

    // there is no reason to manage claims or logins.
    // we simply add or remove them directly on user.
    

    // THE WHOLE STORE IDEA IS COMPLETELY UNNECESASARRY
    // IN THIS INCARNATION WHERE IT ACCEPTS UNIT OF WORK 
    // IT SHOULD BE!! UNIT OF WORK!

    public class UserStore<TUser, TKey, TRole, TClaim, TLogin> : IUserStore<TUser, TKey, TRole, TClaim, TLogin>
        where TUser : class, IUser<TKey, TRole, TClaim, TLogin>
        where TKey : IEquatable<TKey>
        where TRole : class, IRole<TKey>
        where TLogin : class, ILogin<TKey>
        where TClaim : class, IClaim<TKey>
    {

        #region Private Fields

        private readonly IIdentityUnitOfWork<TUser, TKey, TRole, TClaim, TLogin> _UnitOfWork;

        #endregion

        #region Constructor

        public UserStore(IIdentityUnitOfWork<TUser, TKey, TRole, TClaim, TLogin> unitOfWork)
        {
            _UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        #endregion

        #region Properties

        public IIdentityUnitOfWork<TUser, TKey, TRole, TClaim, TLogin> UnitOfWork => _UnitOfWork;

        #endregion

        #region Public Methods

        public virtual Task<TUser> GetByExternalLoginAsync(string provider, string providerId)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(providerId))
            {
                throw new ArgumentNullException(nameof(providerId));
            }

            // it's ok to do this because this combination is unique in the database
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o =>
                    o.Logins.Any(s =>
                        provider.Equals(s.LoginProvider, StringComparison.OrdinalIgnoreCase)
                        &&
                        providerId.Equals(s.ProviderUniqueId, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .FirstOrDefaultAsync();
        }

        public async Task<MembershipResult> DeleteLoginFromUserAsync(TLogin login, TUser user)
        {
            // ensure it belongs
            var userFromDb = await FindByIdUniqueIdAsync(user.UniqueId);

            if (userFromDb == null)
            {
                return MembershipResult.Failed("An error occured.");
            }

            var loginToRemove = user.Logins.SingleOrDefault(o => 
                o.LoginProvider.Equals(login.LoginProvider , StringComparison.Ordinal)
                && 
                o.ProviderUniqueId.Equals(login.ProviderUniqueId, StringComparison.Ordinal)
            );

            if (loginToRemove == null)
            {
                throw new ArgumentException("LOgin must be associated with user.", nameof(login));
            }

            UnitOfWork.LoginRepository.Delete(loginToRemove);
            return await SaveAsync();
        }

        public virtual async Task<MembershipResult> CreateAsync(TUser user)
        {
            user.Created = DateTimeOffset.UtcNow;
            user.Modified = DateTimeOffset.UtcNow;
            UnitOfWork.UserRepository.Insert(user);
            return await SaveAsync();
        }

        public virtual Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .FirstOrDefaultAsync();
        }

        public virtual Task<TUser> FindByIdAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TUser> FindByIdUniqueIdAsync(Guid uniqueId)
        {
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.UniqueId == uniqueId)
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .FirstOrDefaultAsync();
        }

        public virtual Task<TUser> FindByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<MembershipResult> UpdateAsync(TUser user)
        {
            user.Modified = DateTimeOffset.UtcNow;
            UnitOfWork.UserRepository.Update(user);
            return await SaveAsync();
        }

        public virtual async Task<MembershipResult> DeleteAsync(TUser user)
        {
            var userToDelete = await FindByIdUniqueIdAsync(user.UniqueId);

            // logins and claims are cascade
            // roles are not dependent on user.

            // related posts, comments, etc will be deleted in the override. this should be enough for basic users --- not administrators

            UnitOfWork.UserRepository.Delete(userToDelete);
            return await SaveAsync();
        }

        #endregion

        #region Protected Methods

        protected virtual async Task<MembershipResult> SaveAsync() // TODO: Throw once all is prepared. this is shit
        {
            try
            {
                await UnitOfWork.SaveAsync();
                return MembershipResult.Success;
            }
            catch (Exception)
            {
                return MembershipResult.Failed(IdentityCodes.SavingError, "Error Occured" );
            }
        }

        #endregion
    }
}
