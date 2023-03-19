using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IUser : IUser<int, IdentityRole, IdentityClaim, IdentityLogin>
    {
    }
}
