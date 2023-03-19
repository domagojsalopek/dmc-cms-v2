using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityClaim<TKey>
        : IClaim<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id
        {
            get;
            set;
        }

        public TKey UserId
        {
            get;
            set;
        }

        public string ClaimType
        {
            get;
            set;
        }

        public string ClaimValue
        {
            get;
            set;
        }
    }
}
