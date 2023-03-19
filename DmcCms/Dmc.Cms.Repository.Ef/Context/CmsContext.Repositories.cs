using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.Repository.Ef
{
    public partial class CmsContext
    {
        public Guid InstanceId { get; } = Guid.NewGuid();

        // do we need anything in here? we'll use a repository ... 
        // controller should not touch this anyway, so it can't 
        // hurt. controller will work with unit of work or service ...
        //public DbSet<User> Users
        //{
        //    get;
        //    set;
        //}
    }
}
