using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Model
{
    public class CookieConsent : EntityBase
    {
        public CookieConsent()
        {
            
        }

        public Guid UniqueId
        {
            get;
            set;
        }

        public string EncryptedValue
        {
            get;
            set;
        }

        public string IpAddress
        {
            get;
            set;
        }

        public string UserAgent
        {
            get;
            set;
        }

        public string RequestUrl
        {
            get;
            set;
        }

        public bool ConsentGiven // for now we store only if allowed anyway
        {
            get;
            set;
        }

        public DateTimeOffset ConsentGivenDateUtc
        {
            get;
            set;
        }

        // region custom methods

        public string Serialize()
        {
            return string.Join("|", new string[] {
                EncryptedValue,
                UniqueId.ToString(),
                ConsentGivenDateUtc.ToUniversalTime().UtcTicks.ToString(),
                ConsentGiven.ToString()
            });
        }
    }
}
