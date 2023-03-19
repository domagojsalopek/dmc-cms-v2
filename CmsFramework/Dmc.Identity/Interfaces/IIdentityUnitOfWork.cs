using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IIdentityUnitOfWork<TUser> : IIdentityUnitOfWork<TUser, int, IdentityRole, IdentityClaim, IdentityLogin>
        where TUser : class, IUser
    {
    }

    public interface IIdentityUnitOfWork<TUser, TKey, TRole, TClaim, TLogin>
        where TUser : class, IUser<TKey, TRole, TClaim, TLogin>
        where TKey : IEquatable<TKey>
        where TRole : class, IRole<TKey>
        where TLogin : class, ILogin<TKey>
        where TClaim : class, IClaim<TKey>
    {
        IRepository<TUser> UserRepository
        {
            get;
        }

        IRepository<TRole> RoleRepository
        {
            get;
        }

        IRepository<TLogin> LoginRepository
        {
            get;
        }

        Task SaveAsync();
    }
}
