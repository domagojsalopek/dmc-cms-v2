using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web
{
    public enum UserAccountMessageType
    {
        Undefined = 0,
        ExternalLoginEmailAlreadyUsed = 1,
        ExternalLoginIdAlreadyUsed = 2,
        ExternalLoginAdded = 3,
        ExternalLoginRemoved = 4
    }
}