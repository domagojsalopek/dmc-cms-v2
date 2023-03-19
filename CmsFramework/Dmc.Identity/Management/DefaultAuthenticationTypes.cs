namespace Dmc.Identity
{
    // this is copied from identity. we don't use entire identity
    // we have our own users because we might decide to separate
    // admins and users later. it's not that difficult to migrate
    // to identity if needed. we'll probably migrate to identity
    public static class IdentityAuthenticationTypes
    {
        public const string ApplicationCookie = "ApplicationCookie";
        public const string ExternalCookie = "ExternalCookie";
        public const string ExternalBearer = "ExternalBearer";
        public const string TwoFactorCookie = "TwoFactorCookie";
        public const string TwoFactorRememberBrowserCookie = "TwoFactorRememberBrowser";
    }
}