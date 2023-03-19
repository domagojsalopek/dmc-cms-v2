using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Web.Settings;
using Dmc.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web
{
    public abstract class ControllerBase : Controller
    {
        #region Private Fields

        private static readonly TimeSpan _DefaultLoginDuration = TimeSpan.FromDays(7); // TODO: To Configuration
        private IAuthenticationManager _AuthenticationManager;
        private readonly ApplicationUserManager _UserManager;
        private User _LoggedInUser;
        private Guid? _LoggedInUserId;
        private bool _TriedToResolveUserId = false;
        private IAppConfig _AppConfig;

        #endregion

        #region Constructors

        protected ControllerBase(ApplicationUserManager manager) 
            : this(manager, Settings.AppConfig.Instance)
        {

        }

        protected ControllerBase(ApplicationUserManager manager, IAppConfig appConfig)
        {
            _UserManager = manager ?? throw new ArgumentNullException(nameof(manager));
            AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            ViewData[Settings.AppConfig.ViewDataSettingsName] = AppConfig;
        }

        #endregion

        #region Properties

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                if (_AuthenticationManager == null)
                {
                    _AuthenticationManager = HttpContext.GetOwinContext().Authentication;
                }
                return _AuthenticationManager;
            }
        }

        public ApplicationUserManager UserManager => _UserManager;

        public Guid? LoggedInUserId
        {
            get
            {
                if (_TriedToResolveUserId)
                {
                    return _LoggedInUserId;
                }

                TryResolveLoggedInUserId();
                _TriedToResolveUserId = true;

                return _LoggedInUserId;
            }
        }

        public IAppConfig AppConfig { get => _AppConfig; private set => _AppConfig = value; }

        #endregion

        #region Helper Methods

        protected ActionResult NotFound()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("NotFound");
        }

        protected void AddErrorMessagesToViewData(IEnumerable<string> messages)
        {
            List<AppMessage> appMessages = new List<AppMessage>();

            foreach (string message in messages)
            {
                appMessages.Add(new AppMessage(MessageType.Error, message));
            }

            AddMessageToViewData(appMessages);
        }

        protected void AddErrorMessagesToTempData(IEnumerable<string> messages)
        {
            TempData[EnvironmentKeys.DataDictionaryMessagesKey] = messages;
        }

        protected void AddMessageToTempData(MessageType type, string message)
        {
            AddMessageToTempData(new AppMessage(type, message));
        }

        protected void AddMessageToTempData(AppMessage message)
        {
            AddMessageToTempData(new List<AppMessage> { message });
        }

        protected void AddMessageToTempData(IEnumerable<AppMessage> message)
        {
            List<AppMessage> messages = TempData.ContainsKey(EnvironmentKeys.DataDictionaryMessagesKey)
                ? TempData[EnvironmentKeys.DataDictionaryMessagesKey] as List<AppMessage>
                : new List<AppMessage>();

            messages.AddRange(message);
            TempData[EnvironmentKeys.DataDictionaryMessagesKey] = messages;
        }

        protected void AddMessageToViewData(MessageType type, string message)
        {
            AddMessageToViewData(new AppMessage(type, message));
        }

        protected void AddMessageToViewData(AppMessage message)
        {
            AddMessageToViewData(new List<AppMessage> { message });
        }

        protected void AddMessageToViewData(IEnumerable<AppMessage> message)
        {
            List<AppMessage> messages = ViewData.ContainsKey(EnvironmentKeys.DataDictionaryMessagesKey)
                ? ViewData[EnvironmentKeys.DataDictionaryMessagesKey] as List<AppMessage>
                : new List<AppMessage>();

            messages.AddRange(message);
            ViewData[EnvironmentKeys.DataDictionaryMessagesKey] = messages;
        }

        protected void AddToModelStateMessagesFromMembershipResult(MembershipResult result)
        {
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), item);
            }
        }

        protected bool HoneyPotCheckValid(string fieldName)
        {
            return string.IsNullOrWhiteSpace(Request.Params[fieldName]);
        }

        protected EmailClient CreateEmailClient()
        {
            return new EmailClient(AppConfig.EmailSettings.SmtpHost
                , AppConfig.EmailSettings.Username
                , AppConfig.EmailSettings.Password
                , new MailAddress(AppConfig.EmailSettings.SendFromEmail, AppConfig.EmailSettings.SendFromName));
        }

        #endregion

        #region User Helper Methods

        protected Guid? GetLoggedInUserId()
        {
            var user = User.Identity as ClaimsIdentity;

            if (user == null)
            {
                return null;
            }

            var claim = user.Claims
                .FirstOrDefault(o =>
                    o.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)
                );

            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
            {
                return null;
            }

            Guid userId;
            if (!Guid.TryParse(claim.Value, out userId))
            {
                return null;
            }

            return userId;
        }

        protected void TryResolveLoggedInUserId()
        {
            if (_LoggedInUserId.HasValue)
            {
                return;
            }

            if (!User.Identity.IsAuthenticated)
            {
                return;
            }

            _LoggedInUserId = GetLoggedInUserId();
        }

        protected async Task<User> GetLoggedInUserAsync()
        {
            if (_LoggedInUser != null)
            {
                return _LoggedInUser;
            }

            Guid? loggedInUserId = GetLoggedInUserId();

            if (!loggedInUserId.HasValue)
            {
                return null;
            }

            User loggedInUser = await UserManager.FindUserByUniqueIdAsync(loggedInUserId.Value);

            if (loggedInUser == null)
            {
                return null;
            }

            _LoggedInUser = loggedInUser;
            return loggedInUser;
        }

        protected void IdentitySignin(ClaimsIdentity identity, bool isPersistent = false)
        {
            AuthenticationManager.SignIn(new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = isPersistent,
                ExpiresUtc = DateTime.UtcNow.Add(_DefaultLoginDuration),

                IssuedUtc = DateTimeOffset.UtcNow

            }, identity);
        }

        protected void IdentitySignin(LoggedInUserInfo appUserState, bool isPersistent = false)
        {
            var claims = new List<Claim>();

            // create *required* claims
            claims.Add(new Claim(ClaimTypes.NameIdentifier, appUserState.UserId));
            claims.Add(new Claim(ClaimTypes.Name, appUserState.Name));

            // serialized LoggedInUserInfo object
            claims.Add(new Claim(AppConstants.LoggedInUserInfoKey, appUserState.Serialize()));

            // create the identity.
            var identity = new ClaimsIdentity(claims, IdentityAuthenticationTypes.ApplicationCookie);

            IdentitySignin(identity);
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(IdentityAuthenticationTypes.ApplicationCookie, IdentityAuthenticationTypes.ExternalCookie);
        }

        #endregion

        #region Redirect and View Methods

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Default");
        }

        public virtual ActionResult Error()
        {
            return View("Error");
        }

        #endregion
    }
}