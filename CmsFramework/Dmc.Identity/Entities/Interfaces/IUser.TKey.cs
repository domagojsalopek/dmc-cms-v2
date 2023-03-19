using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        TKey Id
        {
            get;
            set;
        }

        Guid UniqueId
        {
            get;
            set;
        }

        string Email
        {
            get;
            set;
        }

        string UserName
        {
            get;
            set;
        }

        byte[] PasswordHash
        {
            get;
            set;
        }

        string SecurityStamp
        {
            get;
            set;
        }

        bool EmailConfirmed
        {
            get;
            set;
        }

        bool TwoFactorEnabled
        {
            get;
            set;
        }

        int LoginFailedCount
        {
            get;
            set;
        }

        bool LockoutEnabled
        {
            get;
            set;
        }

        DateTimeOffset? LockoutEnd
        {
            get;
            set;
        }

        DateTimeOffset Created
        {
            get;
            set;
        }

        DateTimeOffset Modified
        {
            get;
            set;
        }
    }
}
