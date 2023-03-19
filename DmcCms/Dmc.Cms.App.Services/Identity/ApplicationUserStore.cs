using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Identity
{
    public class ApplicationUserStore : UserStore<User>, IService
    {
        #region Constructors

        public ApplicationUserStore(IIdentityUnitOfWork<User> unitOfWork) : base(unitOfWork)
        {
        }

        #endregion

        public Task<User> GetWithAllDetailsAsync(Guid uniqueId)
        {
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.UniqueId == uniqueId)
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .Include(o => o.FavouritePosts)
                .Include(o => o.Ratings)
                .Include(o => o.Comments)
                .Include(o => o.Comments.Select(s => s.Parent))
                .FirstOrDefaultAsync();
        }

        public override Task<User> FindByIdUniqueIdAsync(Guid uniqueId)
        {
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.UniqueId == uniqueId)
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .Include(o => o.FavouritePosts)
                .FirstOrDefaultAsync();
        }

        public override Task<User> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException(nameof(email));
            }
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .Include(o => o.FavouritePosts)
                .FirstOrDefaultAsync();
        }

        public override Task<User> FindByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException(nameof(userName));
            }
            return UnitOfWork.UserRepository
                .Query()
                .Filter(o => o.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                .Include(o => o.Roles)
                .Include(o => o.Claims)
                .Include(o => o.Logins)
                .Include(o => o.FavouritePosts)
                .FirstOrDefaultAsync();
        }
    }
}
