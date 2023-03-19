using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    // this all is a bit too much ... 
    public class IdentityUser<TKey, TRole, TClaim, TLogin> : IUser<TKey, TRole, TClaim, TLogin> 
        where TKey  : IEquatable<TKey>
        where TRole : IRole<TKey>
        where TLogin : ILogin<TKey>
        where TClaim : IClaim<TKey>
    {
        #region Constructors

        public IdentityUser()
        {
            UniqueId = Guid.NewGuid();
            Roles = new List<TRole>();
            Claims = new List<TClaim>();
            Logins = new List<TLogin>();
        }

        #endregion

        #region Properties

        public virtual TKey Id
        {
            get;
            set;
        }

        public Guid UniqueId
        {
            get;
            set;
        }

        public virtual string Email
        {
            get;
            set;
        }

        public virtual string UserName
        {
            get;
            set;
        }

        public virtual byte[] PasswordHash
        {
            get;
            set;
        }

        public virtual string SecurityStamp
        {
            get;
            set;
        }

        public virtual bool EmailConfirmed
        {
            get;
            set;
        }

        public virtual bool TwoFactorEnabled
        {
            get;
            set;
        }

        public virtual bool LockoutEnabled
        {
            get;
            set;
        }

        public int LoginFailedCount
        {
            get;
            set;
        }

        public virtual DateTimeOffset? LockoutEnd
        {
            get;
            set;
        }

        public DateTimeOffset Created
        {
            get;
            set;
        }

        public DateTimeOffset Modified
        {
            get;
            set;
        }

        #endregion

        #region Dependent Things

        public virtual ICollection<TClaim> Claims
        {
            get;
            private set;
        }

        public virtual ICollection<TLogin> Logins
        {
            get;
            private set;
        }

        public virtual ICollection<TRole> Roles
        {
            get;
            private set;
        }

        #endregion
    }
}
