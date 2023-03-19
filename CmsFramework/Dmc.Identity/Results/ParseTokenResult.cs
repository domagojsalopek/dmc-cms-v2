using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    internal class ParseTokenResult
    {
        internal DateTime Created
        {
            get;
            set;
        }

        internal Guid UniqueId
        {
            get;
            set;
        }

        internal string SecurityStamp
        {
            get;
            set;
        }

        internal string Purpose
        {
            get;
            set;
        }

        internal byte[] Signature
        {
            get;
            set;
        }
    }
}
