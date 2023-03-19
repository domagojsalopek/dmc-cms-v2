using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityLogin : IdentityLogin<int>, ILogin, IEntity
    {
        #region Constructors

        public IdentityLogin()
        {

        }

        #endregion

        public override int Id
        {
            get => base.Id;
            set => base.Id = value;
        }
    }
}
