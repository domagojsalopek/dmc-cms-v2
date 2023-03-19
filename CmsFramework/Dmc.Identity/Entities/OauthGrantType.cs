using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    [Flags]
    public enum OauthGrantType
    {
        None = 0,
        ClientCredentials = 1,
        AuthorizationCode = 2,
        ResourceOwner = 4
    }
}
