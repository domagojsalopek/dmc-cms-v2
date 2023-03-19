using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    public partial class CmsContext : DbContext
    {
        #region Constructors

        public CmsContext() 
            : base("DefaultConnection")
        {

        }

        //public CmsContext(string nameOrConnectionString) 
        //    : base(nameOrConnectionString)
        //{

        //}

        #endregion
    }
}
