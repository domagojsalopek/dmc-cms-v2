using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace Dmc.Cms.Web.Oauth
{
    // https://stackoverflow.com/questions/24340088/adding-additional-logic-to-bearer-authorization
    public class CmsOauthServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IApplicationUserManagerFactory _UserManagerFactory;
        private readonly ICmsUnitOfWorkFactory _CmsUnitOfWorkFactory;

        public CmsOauthServerProvider(IApplicationUserManagerFactory applicationUserManagerFactory, ICmsUnitOfWorkFactory cmsUnitOfWorkFactory) 
        {
            _UserManagerFactory = applicationUserManagerFactory ?? throw new ArgumentNullException(nameof(applicationUserManagerFactory));
            _CmsUnitOfWorkFactory = cmsUnitOfWorkFactory ?? throw new ArgumentNullException(nameof(cmsUnitOfWorkFactory));
        }

        #region Client Authentication

        // TODO: user ... 

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret) && 
                !context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                context.Rejected();
                return;
            }

            // TODO: separate to be able to return server error
            if (await TryAuthenticateClient(clientId, clientSecret))
            {
                context.Validated();
                return;
            }

            context.Rejected();
        }

        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            var identity = new ClaimsIdentity(new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));
            context.Validated(identity);

            return Task.FromResult(0);
        }

        #endregion

        #region Helper Methods

        private async Task<bool> TryAuthenticateClient(string clientId, string clientSecret)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                return false;
            }

            try
            {
                using (var uow = _CmsUnitOfWorkFactory.Create())
                {
                    var app = await uow.AppRepository.GetByClientIdAsync(clientId);

                    if (app == null)
                    {
                        return false;
                    }

                    if (!app.ClientSecret.Equals(clientSecret, StringComparison.Ordinal))
                    {
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}