using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IUser<TKey, TRole, TClaim, TLogin> : IUser<TKey>
        where TKey : IEquatable<TKey>
        where TRole : IRole<TKey>
        where TLogin : ILogin<TKey>
        where TClaim : IClaim<TKey>
    {
        ICollection<TClaim> Claims
        {
            get;
        }

        ICollection<TLogin> Logins
        {
            get;
        }

        ICollection<TRole> Roles
        {
            get;
        }
    }
}
