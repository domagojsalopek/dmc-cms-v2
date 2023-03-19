using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Dmc.Identity.Ef
{
    public class UserRepository<T> : Repository<T> where T : class, IUser
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}
