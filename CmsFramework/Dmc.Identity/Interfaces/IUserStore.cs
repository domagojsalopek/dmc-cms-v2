using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IUserStore<T> : IUserStore<T, int, IdentityRole, IdentityClaim, IdentityLogin>
        where T : IUser
    {
    }

    public interface IUserStore<T, TKey, TRole, TClaim, TLogin> 
        where T : IUser<TKey, TRole, TClaim, TLogin>
        where TKey : IEquatable<TKey>
        where TRole : IRole<TKey>
        where TLogin : ILogin<TKey>
        where TClaim : IClaim<TKey>
    {
        Task<MembershipResult> DeleteLoginFromUserAsync(TLogin login, T user);

        Task<T> GetByExternalLoginAsync(string provider, string providerId);

        Task<T> FindByIdAsync(TKey id);

        Task<T> FindByIdUniqueIdAsync(Guid uniqueId);

        Task<T> FindByUserNameAsync(string userName);

        Task<T> FindByEmailAsync(string email);

        Task<MembershipResult> CreateAsync(T user);

        Task<MembershipResult> UpdateAsync(T user);

        Task<MembershipResult> DeleteAsync(T user);
    }
}
