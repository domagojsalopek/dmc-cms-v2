using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public static class SettingKeys
    {
        public static readonly string SocialFacebook = "Social.Facebook";
        public static readonly string SocialTwitter = "Social.Twitter";
        public static readonly string SocialInstagram = "Social.Instagram";

        public static readonly string EmailSmtpServer = "Email.SMTPServer";
        public static readonly string EmailSmtpPort = "Email.SMTPPort";
        public static readonly string EmailSmtpUsername = "Email.SMTPUsername";
        public static readonly string EmailSmtpPassword = "Email.SMTPPassword";
        public static readonly string EmailSendFromName = "Email.SendFromName";
        public static readonly string EmailSendFromEmail = "Email.SendFromEmail";

        public static readonly string SiteSettingsDescription = "SiteSettings.Description";
        public static readonly string SiteSettingsName = "SiteSettings.Name";
        public static readonly string SiteSettingsLatestUserFavouritePosts = "SiteSettings.LatestUserFavouritePosts";
        public static readonly string SiteSettingsMainContactEmail = "SiteSettings.MainContactEmail";
        public static readonly string SiteSettingsNumberOfRecentPosts = "SiteSettings.NumberOfRecentPosts";
        public static readonly string SiteSettingPostsPerPage = "SiteSettings.PostsPerPage";

        public static readonly string SiteSettingsAnalyticsCode = "SiteSettings.AnalyticsCode";
        public static readonly string SiteSettingsIsAnalyticsEnabled = "SiteSettings.IsAnalyticsEnabled";
        public static readonly string SiteSettingsCustomCookieSolutionCode = "SiteSettings.CustomCookieSolutionCode";
        public static readonly string SiteSettingsIsCustomCookieSolutionEnabled = "SiteSettings.IsCustomCookieSolutionEnabled";
        public static readonly string SiteSettingsAdditionalCustomHeadContent = "SiteSettings.AdditionalCustomHeadContent";
        public static readonly string SiteSettingsCustomCookieAndPrivacyPolicyLinks = "SiteSettings.CustomCookieAndPrivacyPolicyLinks";


        public static readonly string ContentSettingsIsAdvertisingEnabled = "Content.IsAdvertisingEnabled";
        public static readonly string ContentSettingsIsShowingEventsEnabled = "Content.IsShowingEventsEnabled";

        public static readonly string ReCaptchaUseRecaptcha = "ReCaptcha.UseRecaptcha";
        public static readonly string ReCaptchaSiteKey = "ReCaptcha.SiteKey";
        public static readonly string ReCaptchaSecret = "ReCaptcha.Secret";

        public const string Resources_EmailHeader = "Resources.EmailHeader";
        public const string Resources_Logo = "Resources.Logo";
        public const string Resources_RetinaLogo = "Resources.RetinaLogo";
        public const string Resources_FooterLogo = "Resources.FooterLogo";
        public const string Resources_DefaultSiteHeader = "Resources.DefaultSiteHeader";
        public const string Resources_DefaultOpenGraphImage = "Resources.DefaultOpenGraphImage";


        public static readonly string ExternalLoginProviderNameFormat = "ExternalLogin[{0}].Name";
        public static readonly string ExternalLoginProviderClientIdFormat = "ExternalLogin[{0}].ClientId";
        // public static readonly string ExternalLoginProviderSecretFormat = "ExternalLogin[{0}].ClientSecret"; not yet needed
    }
}
