using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityLogin<TKey> : ILogin<TKey>
        where TKey : IEquatable<TKey>
    {
        #region Constructors

        public IdentityLogin()
        {

        }

        #endregion

        #region Properties

        public virtual TKey Id { get; set; }

        public TKey UserId { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderUniqueId { get; set; }

        public virtual string ScreenName { get; set; }

        #endregion
    }
}
