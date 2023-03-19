using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public class ExternalLoginSettings
    {
        private static readonly IReadOnlyCollection<string> _SupportedLoginIds = new ReadOnlyCollection<string>(new List<string>()
        {
            ExternalLoginKeys.Amazon,
            ExternalLoginKeys.Facebook
        });

        public ExternalLoginSettings(IList<ExternalLogin> logins)
        {
            ExternalLogins = new ReadOnlyCollection<ExternalLogin>(logins);
        }

        public static IReadOnlyCollection<string> SupportedExternalLoginIds => _SupportedLoginIds;

        public IEnumerable<ExternalLogin> ExternalLogins { get; }

        public ExternalLogin GetExternalLogin(string identifier)
        {
            return ExternalLogins?.FirstOrDefault(o => identifier.Equals(o.ServiceIdentifier, StringComparison.OrdinalIgnoreCase));
        }
    }
}
