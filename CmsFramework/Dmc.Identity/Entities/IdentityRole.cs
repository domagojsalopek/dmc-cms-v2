using Dmc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class IdentityRole : IdentityRole<int>, IRole, IEntity
    {
        #region Constructors

        public IdentityRole()
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
