using Dmc.Repository.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Dmc.Repository;

namespace Dmc.Identity.Ef
{
    public class IdentityUnitOfWork<T> : UnitOfWork, IIdentityUnitOfWork<T> where T : class, IUser
    {
        private IRepository<T> _UserRepository;
        private IRepository<IdentityRole> _RoleRepository;
        private IRepository<IdentityLogin> _LoginRepository;

        public IdentityUnitOfWork(DbContext context) 
            : base(context)
        {
        }

        public IRepository<T> UserRepository
        {
            get
            {
                if (_UserRepository == null)
                {
                    _UserRepository = new UserRepository<T>(Context);
                }
                return _UserRepository;
            }
        }

        public IRepository<IdentityRole> RoleRepository
        {
            get
            {
                if (_RoleRepository == null)
                {
                    _RoleRepository = new RoleRepository(Context);
                }
                return _RoleRepository;
            }
        }

        public IRepository<IdentityLogin> LoginRepository
        {
            get
            {
                if (_LoginRepository == null)
                {
                    _LoginRepository = new Repository<IdentityLogin>(Context);
                }
                return _LoginRepository;
            }
        }
    }
}
