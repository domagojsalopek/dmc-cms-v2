using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Identity
{
    public class ApplicationUserManager : UserManager<User>, IService
    {
        #region Private Fields

        private ICmsUnitOfWork _CmsUnitOfWork;
        private static readonly TimeSpan _DefaultLoginDuration = TimeSpan.FromDays(7); // TODO: to configuration

        #endregion

        #region Constructors

        public ApplicationUserManager(ApplicationUserStore userStore, ICmsUnitOfWork cmsUnitOfWork) 
            : base(userStore)
        {
            _CmsUnitOfWork = cmsUnitOfWork;
        }

        #endregion

        #region Helper Methods

        public async Task<User> GetUserWithAllDetailsAsync(Guid uniqueId)
        {
            ApplicationUserStore store = UserStore as ApplicationUserStore;
            return await store.GetWithAllDetailsAsync(uniqueId);
        }

        #endregion  

        #region Identity

        public override async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            // will create NameIdentifier and roles
            var identity = await base.CreateIdentityAsync(user, authenticationType);

            LoggedInUserInfo appUserState = new LoggedInUserInfo();
            appUserState.FromUser(user);
            appUserState.ExpiresUtc = DateTimeOffset.UtcNow.Add(_DefaultLoginDuration);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            identity.AddClaim(new Claim(AppConstants.LoggedInUserInfoKey, appUserState.Serialize()));

            return identity;
        }

        #endregion

        #region Additional Overrides

        public override async Task<MembershipResult> DeleteUserAsync(User user, string token)
        {
            var userToDelete = await GetUserWithAllDetailsAsync(user.UniqueId);

            // it's OK. won't be saved if token is bad
            AnonymiseComments(userToDelete);
            DeleteRatings(userToDelete);
            await DeleteQueries(userToDelete);

            return await base.DeleteUserAsync(userToDelete, token);
        }

        private void AnonymiseComments(User userToDelete)
        {
            if (userToDelete.Comments == null)
            {
                return;
            }

            foreach (Comment comment in userToDelete.Comments)
            {
                comment.Author = "Anonymous"; // we need to do this. user said delete account. cannot keep personally identifiable info
                comment.Status = CommentStatus.Anonymised;

                comment.User = null;
                comment.UserId = null;
                comment.Email = null;
            }
        }

        private async Task DeleteQueries(User userToDelete)
        {
            var queriesToChange = await _CmsUnitOfWork.ContactRepository.GetAllForUserAsync(userToDelete.Id);

            foreach (var item in queriesToChange)
            {
                //item.UserId = null; // deassociate
                _CmsUnitOfWork.ContactRepository.Delete(item);
            }
        }

        private void DeleteRatings(User userToDelete)
        {
            var ratingsToDelete = new List<Rating>();
            ratingsToDelete.AddRange(userToDelete.Ratings);

            foreach (var item in ratingsToDelete)
            {
                _CmsUnitOfWork.RatingRepository.Delete(item);
            }
        }

        #endregion
    }
}
