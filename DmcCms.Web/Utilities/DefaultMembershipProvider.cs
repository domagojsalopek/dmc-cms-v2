using Dmc.Cms.App;
using Dmc.Cms.Model;
using Dmc.Cms.Repository.Ef;
using Dmc.Identity;
using Dmc.Identity.Ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Dmc.Cms.Web
{
    internal static class DefaultMembershipProvider
    {
        private static bool _AlreadyTried = false;
        private static readonly SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);

        internal static async Task InitializeMembershipIfNeeded()
        {
            if (_AlreadyTried)
            {
                return;
            }

            string userName = WebConfigurationManager.AppSettings["DefaultAdminUsername"];

            if (string.IsNullOrWhiteSpace(userName)) // nothing to do.
            {
                _AlreadyTried = true;
                return;
            }

            try
            {
                await _Semaphore.WaitAsync();

                if (_AlreadyTried)
                {
                    return;
                }

                CmsContext context = new CmsContext();
                UserManager<User> userManager = new UserManager<User>(new UserStore<User>(new IdentityUnitOfWork<User>(context)));

                var existingUser = await userManager.FindUserByUserNameAsync(userName);
                if (existingUser != null)
                {
                    _AlreadyTried = true;
                    return;
                }

                await CreateDefaultUserAndAddToRole(userManager, userName, context);
            }
            finally
            {
                _Semaphore.Release();
            }
        }

        private static async Task CreateDefaultUserAndAddToRole(UserManager<User> userManager, string userName, CmsContext context)
        {
            User userTOSave = new User
            {
                FirstName = "Default",
                LastName = "Admin",
                Email = userName,
                UserName = userName,
                EmailConfirmed = true,
                NickName = WebConfigurationManager.AppSettings["DefaultAdminNickname"]
            };

            var userCreateResult = await userManager.CreateUserAsync(userTOSave, WebConfigurationManager.AppSettings["DefaultAdminPassword"]);

            if (!userCreateResult.Succeeded)
            {
                return;
            }

            const string roleName = RoleKeys.Admin;
            IdentityRole defaultRole = await GetExistingOrCreateNewRoleIfNeeded(context, roleName);

            if (defaultRole == null)
            {
                return;
            }

            await userManager.AddRoleToUser(userTOSave, defaultRole);
        }

        private static async Task<IdentityRole> GetExistingOrCreateNewRoleIfNeeded(CmsContext context, string roleName)
        {
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<User, IdentityRole>(new IdentityUnitOfWork<User>(context)));
            var role = await roleManager.FindByName(roleName);

            if (role != null)
            {
                return role;
            }

            var result = await roleManager.CreateRoleIfNotExists(roleName);
            if (!result.Succeeded)
            {
                return null;
            }

            return await roleManager.FindByName(roleName);
        }
    }
}