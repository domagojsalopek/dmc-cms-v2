using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Dmc.Identity.Ef
{
    public class RoleRepository : Repository<IdentityRole>
    {
        public RoleRepository(DbContext context) : base(context)
        {
        }
    }
}
