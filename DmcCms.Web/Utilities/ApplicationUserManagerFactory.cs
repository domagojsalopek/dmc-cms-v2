using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Repository.Ef;
using Dmc.Identity.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web
{
    public class ApplicationUserManagerFactory : IApplicationUserManagerFactory
    {
        public ApplicationUserManager Create()
        {
            var dbContext = new CmsContext();
            return new ApplicationUserManager(new ApplicationUserStore(new IdentityUnitOfWork<User>(dbContext)), new CmsUnitOfWork(dbContext));
        }
    }
}