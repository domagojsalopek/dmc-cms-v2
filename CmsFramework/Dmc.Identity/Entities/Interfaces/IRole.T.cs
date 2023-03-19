using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IRole<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }
    }
}
