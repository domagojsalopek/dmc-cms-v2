using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Identity
{
    public class RoleManager<T> : RoleManager<T, int>
        where T : IRole, new()
    {
        public RoleManager(IRoleStore<T, int> roleStore) : base(roleStore)
        {
        }
    }

    public class RoleManager<T, TKey> 
        where T : IRole<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        #region Private Fields

        private readonly IRoleStore<T, TKey> _RoleStore;

        #endregion

        #region Constructors

        public RoleManager(IRoleStore<T, TKey> roleStore)
        {
            _RoleStore = roleStore ?? throw new ArgumentNullException(nameof(roleStore));
        }

        #endregion

        #region Methods

        public Task<T> FindByName(string name)
        {
            return _RoleStore.FindByNameAsync(name);
        }

        public async Task<MembershipResult> CreateRoleIfNotExists(string roleName)
        {
            T existingRole = await _RoleStore.FindByNameAsync(roleName);

            if (existingRole != null)
            {
                return MembershipResult.Success; // nothing to do.
            }

            // create new and save ...
            T role = new T();
            role.Name = roleName;
            return await _RoleStore.CreateAsync(role);
        }

        #endregion
    }
}
