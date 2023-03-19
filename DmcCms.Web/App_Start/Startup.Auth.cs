using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Repository.Ef;
using Dmc.Cms.Web.Oauth;
using Dmc.Cms.Web.Settings;
using Dmc.Identity;
using Dmc.Identity.Ef;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace Dmc.Cms.Web
{
    public partial class Startup
    {
        private const int ValidateIntervalInMinutes = 15;

        public void ConfigureAuth(IAppBuilder app)
        {
            // setup cookie
            SetupDefaultAuthnetication(app);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // setup oauth
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                AuthorizeEndpointPath = new PathString("/oauth/authorize"),
                TokenEndpointPath = new PathString("/oauth/token"),
                ApplicationCanDisplayErrors = true,
#if DEBUG
                AllowInsecureHttp = true,
#endif
                // Authorization server provider which controls the lifecycle of Authorization Server
                //Provider = new OAuthAuthorizationServerProvider
                //{
                //    OnValidateClientAuthentication = ValidateClientAuthentication,
                //    OnGrantClientCredentials = GrantClientCredetails,
                //},
                Provider = new CmsOauthServerProvider(new ApplicationUserManagerFactory(), new CmsUnitOfWorkFactory()),

                // Refresh token provider which creates and receives referesh token
                //RefreshTokenProvider = new AuthenticationTokenProvider
                //{
                //    OnCreate = CreateRefreshToken,
                //    OnReceive = ReceiveRefreshToken,
                //}

                // let's not use it as client credentials do not demand it - and the spec even says SHOULD NOT
                // RefreshTokenProvider = new RefreshTokenProvider()
            });

            app.Use(async (context, next) =>
            {
                await DefaultMembershipProvider.InitializeMembershipIfNeeded();
                await next.Invoke();
            });
        }

        //private Task GrantClientCredetails(OAuthGrantClientCredentialsContext context)
        //{
        //    var identity = new ClaimsIdentity(new GenericIdentity(context.ClientId, OAuthDefaults.AuthenticationType), context.Scope.Select(x => new Claim("urn:oauth:scope", x)));

        //    context.Validated(identity);

        //    return Task.FromResult(0);
        //}

        //private Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        //{
        //    string clientId;
        //    string clientSecret;
        //    if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
        //        context.TryGetFormCredentials(out clientId, out clientSecret))
        //    {
        //        if (clientId == "123456")
        //        {
        //            context.Validated();
        //        }
        //        else if (clientId == "2")
        //        {
        //            context.Validated();
        //        }
        //    }
        //    return Task.FromResult(0);
        //}

        private static void SetupDefaultAuthnetication(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = IdentityAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = AppConfig.Instance.GetLoginDuration(),
                SlidingExpiration = false,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = o => ApplicationUserValidator.ValidateIdentity(o, TimeSpan.FromMinutes(ValidateIntervalInMinutes), new ApplicationUserManagerFactory())
                }
            });
        }
    }
}