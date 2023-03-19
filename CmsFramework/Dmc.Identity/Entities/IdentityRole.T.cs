using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityRole<TKey> : IRole<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual TKey Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }
    }
}
