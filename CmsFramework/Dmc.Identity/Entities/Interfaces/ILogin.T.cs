using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface ILogin<TKey> 
        where TKey : IEquatable<TKey>
    {
        #region Properties

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

        string LoginProvider // facebook, google
        {
            get;
            set;
        }

        string ProviderUniqueId // id of user on facebook
        {
            get;
            set;
        }

        string ScreenName // what user calls himself there
        {
            get;
            set;
        }

        #endregion
    }
}
