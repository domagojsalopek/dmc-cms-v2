using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class ExternalLogin
    {
        public ExternalLogin(string identifier, string clientId, string name)
        {
            ServiceIdentifier = identifier;
            ClientId = clientId;
            Name = name;
        }

        public string ServiceIdentifier { get; internal set; }

        public string ClientId { get; internal set; }

        public string Name { get; internal set; }
    }
}
