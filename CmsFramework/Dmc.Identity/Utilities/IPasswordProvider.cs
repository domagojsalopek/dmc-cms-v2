using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public interface IPasswordProvider
    {
        #region Methods

        byte[] HashPassword(string password);

        bool VerifyPassword(byte[] password, string plainTextPassword);

        #endregion
    }
}
