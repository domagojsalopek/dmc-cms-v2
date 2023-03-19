using Dmc.Cms.Model;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    internal class CookieConsentRepository : EntityRepositoryBase<CookieConsent>, ICookieConsentRepository
    {
        public CookieConsentRepository(IRepository<CookieConsent> repository) : base(repository)
        {
        }
    }
}
