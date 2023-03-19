using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Dmc.Cms.Repository.Ef
{
    public class RoleRepository : Dmc.Repository.Ef.Repository<IdentityRole>
    {
        public RoleRepository(DbContext context) : base(context)
        {
        }
    }
}
