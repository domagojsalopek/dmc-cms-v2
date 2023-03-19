using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IClaim<TKey> 
        where TKey : IEquatable<TKey>
    {
        TKey Id
        {
            get;
            set;
        }

        TKey UserId
        {
            get;
            set;
        }

        string ClaimType
        {
            get;
            set;
        }

        string ClaimValue
        {
            get;
            set;
        }
    }
}
