using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityUser : IdentityUser<int, IdentityRole, IdentityClaim, IdentityLogin>, IUser, IEntity
    {
        #region Constructors

        public IdentityUser()
        {

        }

        #endregion
    }
}
