using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Cookies;
using System.Security.Claims;
using Dmc.Cms.App;
using Dmc.Cms.Repository.Ef;
using Dmc.Cms.Model;

namespace Dmc.Cms.Web
{
    public class ApplicationUserValidator
    {
        internal static async Task ValidateIdentity(CookieValidateIdentityContext context, TimeSpan validateInterval, ApplicationUserManagerFactory managerFactory)
        {
            var currentUtc = DateTimeOffset.UtcNow;
            var issuedUtc = context.Properties.IssuedUtc;

            bool validate = (issuedUtc == null);

            if (issuedUtc != null)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                validate = timeElapsed > validateInterval;
            }

            if (!validate)
            {
                return;
            }

            string userId = GetUserIdFromClaims(context);

            if (string.IsNullOrWhiteSpace(userId))
            {
                RejectAndSignOut(context);
                return;
            }

            LoggedInUserInfo userInfo = GetUserInfoFromClaims(context);

            if (userInfo == null || !userInfo.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                RejectAndSignOut(context);
                return;
            }

            if (DateTimeOffset.UtcNow > userInfo.ExpiresUtc)
            {
                RejectAndSignOut(context);
                return;
            }

            Guid userIdAsGuid;
            if (!Guid.TryParse(userId, out userIdAsGuid))
            {
                RejectAndSignOut(context);
                return;
            }

            var manager = managerFactory.Create();

            if (manager == null)
            {
                RejectAndSignOut(context);
                return;
            }

            User userFromDb = await manager.FindUserByUniqueIdAsync(userIdAsGuid);

            if (userFromDb == null || string.IsNullOrWhiteSpace(userFromDb.SecurityStamp))
            {
                RejectAndSignOut(context);
                return;
            }

            if (!userFromDb.SecurityStamp.Equals(userInfo.SecurityStamp, StringComparison.OrdinalIgnoreCase))
            {
                RejectAndSignOut(context);
                return;
            }

            var identity = await manager.CreateIdentityAsync(userFromDb, context.Options.AuthenticationType);

            if (identity == null)
            {
                RejectAndSignOut(context);
                return;
            }

            context.OwinContext.Authentication.SignIn(context.Properties, identity);
        }

        private static void RejectAndSignOut(CookieValidateIdentityContext context)
        {
            context.RejectIdentity();
            context.OwinContext.Authentication.SignOut(context.Options.AuthenticationType);
        }

        private static LoggedInUserInfo GetUserInfoFromClaims(CookieValidateIdentityContext context)
        {
            try
            {
                var claim = context.Identity.Claims.FirstOrDefault(o => AppConstants.LoggedInUserInfoKey.Equals(o.Type, StringComparison.OrdinalIgnoreCase));

                if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                {
                    return null;
                }

                // currently this is not encrypted. should it be ??

                LoggedInUserInfo info = new LoggedInUserInfo();

                if (!info.FromString(claim.Value))
                {
                    return null;
                }

                return info;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetUserIdFromClaims(CookieValidateIdentityContext context)
        {
            var claim = context.Identity.Claims.FirstOrDefault(o => ClaimTypes.NameIdentifier.Equals(o.Type, StringComparison.OrdinalIgnoreCase));

            if (claim == null)
            {
                return null;
            }

            return claim.Value;
        }
    }
}