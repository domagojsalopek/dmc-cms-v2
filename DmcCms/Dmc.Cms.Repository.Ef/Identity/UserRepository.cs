using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Dmc.Cms.Repository.Ef
{
    public class UserRepository : Dmc.Repository.Ef.Repository<User>
    {
        public UserRepository(DbContext context) 
            : base(context)
        {
        }
    }
}
