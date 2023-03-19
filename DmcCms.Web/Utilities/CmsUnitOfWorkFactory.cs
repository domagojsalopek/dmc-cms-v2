using Dmc.Cms.Repository;
using Dmc.Cms.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web
{
    public class CmsUnitOfWorkFactory : ICmsUnitOfWorkFactory
    {
        public ICmsUnitOfWork Create()
        {
            return new CmsUnitOfWork(new CmsContext());
        }
    }
}