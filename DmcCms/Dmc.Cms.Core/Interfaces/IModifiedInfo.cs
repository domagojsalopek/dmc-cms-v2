using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Core
{
    public interface IModifiedInfo
    {
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
