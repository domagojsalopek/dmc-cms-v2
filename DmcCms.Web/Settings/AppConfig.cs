using Dmc.Cms.App;
using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Dmc.Cms.Web.Settings
{
    public class AppConfig : IAppConfig
    {
        // TODO https://blog.stephencleary.com/2012/08/asynchronous-lazy-initialization.html

        public const string ViewDataSettingsName = "AppConfig";
        private const string CacheKeyName = "AppConfigDictionary";

        #region Lazy

        private readonly object _Locker = new object();
        private static Func<IConfigService> _DbContextFactory;
        private static Lazy<AppConfig> _Lazy = CreateLazy();

        #endregion

        #region Constructors

        private AppConfig()
        {
            SocialLinks = new SocialLinks();
            EmailSettings = new EmailSettings();
            SiteSettings = new SiteSettings();
            ContentSettings = new ContentSettings();
            SiteResources = new SiteResources();
            ReCaptchaSettings = new ReCaptchaSettings();

            // start
            Configure();
        }

        #endregion

        #region Singleton Instance

        public static IAppConfig Instance
        {
            get
            {
                return _Lazy.Value;
            }
        }

        #endregion

        #region Properties

        public ISocialLinks SocialLinks
        {
            get;
            private set;
        }

        public IEmailSettings EmailSettings
        {
            get;
            private set;
        }

        public ISiteSettings SiteSettings
        {
            get;
            private set;
        }

        public IContentSettings ContentSettings
        {
            get;
            private set;
        }

        public ISiteResources SiteResources
        {
            get;
            private set;
        }

        public IReCaptchaSettings ReCaptchaSettings 
        { 
            get; 
            private set; 
        }

        public ExternalLoginSettings ExternalLoginSettings
        {
            get;
            private set;
        }

        #endregion

        #region Private Methods

        private void Configure()
        {
            using (var service = _DbContextFactory())
            {
                CacheHelper.RemoveFromCache(CacheKeyName);
                Dictionary<string, string> allOptionsDictionary = CreateOptionsDictionary(service);

                if (allOptionsDictionary == null || allOptionsDictionary.Count <= 0)
                {
                    return;
                }

                CacheHelper.AddToCache(CacheKeyName, allOptionsDictionary);

                // Create known things
                ConfigureSiteSettings(allOptionsDictionary);
                ConfigureSocial(allOptionsDictionary);
                ConfigureEmailSettings(allOptionsDictionary);
                ConfigureContentSettings(allOptionsDictionary);
                ConfigureSiteResources(allOptionsDictionary);
                ConfigureReCaptcha(allOptionsDictionary);

                // use service. todo: entire cms is baaaad
                ExternalLoginSettings = service.GetExternalLoginSettings();
            }            
        }

        private static Dictionary<string, string> CreateOptionsDictionary(IConfigService context)
        {
            IEnumerable<Option> allOptions = context.GetAllOptions(); 

            if (allOptions == null)
            {
                return null;
            }

            return allOptions.ToDictionary(k => k.Name, v => v.Value);
        }

        private void ConfigureSiteSettings(Dictionary<string, string> allOptions)
        {
            var siteSettings = new SiteSettings
            {
                Description = GetValueIfExists(SettingKeys.SiteSettingsDescription, allOptions),
                MainContactEmail = GetValueIfExists(SettingKeys.SiteSettingsMainContactEmail, allOptions),
                Name = GetValueIfExists(SettingKeys.SiteSettingsName, allOptions),

                AnalyticsCode = GetValueIfExists(SettingKeys.SiteSettingsAnalyticsCode, allOptions),
                CustomCookieSolutionCode = GetValueIfExists(SettingKeys.SiteSettingsCustomCookieSolutionCode, allOptions),
                AdditionalCustomHeadContent = GetValueIfExists(SettingKeys.SiteSettingsAdditionalCustomHeadContent, allOptions),
                CustomCookieAndPrivacyPolicyLinks = GetValueIfExists(SettingKeys.SiteSettingsCustomCookieAndPrivacyPolicyLinks, allOptions)
            };

            SetNumericValuesOrDefaultsToSiteSettings(siteSettings, allOptions);

            SiteSettings = siteSettings;
        }

        private void ConfigureSiteResources(Dictionary<string, string> allOptionsDictionary)
        {
            var resources = new SiteResources
            {
                RetinaLogo = GetValueIfExists(SettingKeys.Resources_RetinaLogo, allOptionsDictionary),
                DefaultOpenGraphImage = GetValueIfExists(SettingKeys.Resources_DefaultOpenGraphImage, allOptionsDictionary),
                DefaultSiteHeader = GetValueIfExists(SettingKeys.Resources_DefaultSiteHeader, allOptionsDictionary),
                EmailHeader = GetValueIfExists(SettingKeys.Resources_EmailHeader, allOptionsDictionary),
                FooterLogo = GetValueIfExists(SettingKeys.Resources_FooterLogo, allOptionsDictionary),
                Logo = GetValueIfExists(SettingKeys.Resources_Logo, allOptionsDictionary),
            };

            SiteResources = resources;
        }

        private void ConfigureContentSettings(Dictionary<string, string> allOptionsDictionary)
        {
            ContentSettings = new ContentSettings
            {
                IsAdvertisingEnabled = ParseBoolOrDefault(GetValueIfExists(SettingKeys.ContentSettingsIsAdvertisingEnabled, allOptionsDictionary), false),
                IsShowingEventsEnabled = ParseBoolOrDefault(GetValueIfExists(SettingKeys.ContentSettingsIsShowingEventsEnabled, allOptionsDictionary), false)
            };
        }

        private void ConfigureReCaptcha(Dictionary<string, string> allOptionsDictionary)
        {
            ReCaptchaSettings = new ReCaptchaSettings
            {
                UseRecaptcha = ParseBoolOrDefault(GetValueIfExists(SettingKeys.ReCaptchaUseRecaptcha, allOptionsDictionary), false),
                RecaptchaSecretKey = GetValueIfExists(SettingKeys.ReCaptchaSecret, allOptionsDictionary),
                RecaptchaSiteKey = GetValueIfExists(SettingKeys.ReCaptchaSiteKey, allOptionsDictionary)
            };
        }

        private void SetNumericValuesOrDefaultsToSiteSettings(SiteSettings siteSettings, Dictionary<string, string> allOptions)
        {
            siteSettings.LatestUserFavouritePosts = ParseIntOrDefault(GetValueIfExists(SettingKeys.SiteSettingsLatestUserFavouritePosts, allOptions), 3);
            siteSettings.NumberOfRecentPostsToShow = ParseIntOrDefault(GetValueIfExists(SettingKeys.SiteSettingsNumberOfRecentPosts, allOptions), 5);
            siteSettings.PostsPerPage = ParseIntOrDefault(GetValueIfExists(SettingKeys.SiteSettingPostsPerPage, allOptions), 10);

            siteSettings.IsCustomCookieSolutionEnabled = ParseBoolOrDefault(GetValueIfExists(SettingKeys.SiteSettingsIsCustomCookieSolutionEnabled, allOptions), false);
            siteSettings.IsAnalyticsEnabled = ParseBoolOrDefault(GetValueIfExists(SettingKeys.SiteSettingsIsAnalyticsEnabled, allOptions), false);
        }

        private bool ParseBoolOrDefault(string setting, bool defaultValue)
        {
            if (bool.TryParse(setting, out bool parsed))
            {
                return parsed;
            }

            return defaultValue;
        }

        private int ParseIntOrDefault(string setting, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(setting))
            {
                return defaultValue;
            }

            if (int.TryParse(setting, out int valueToReturn))
            {
                return valueToReturn;
            }

            return defaultValue;
        }

        private void ConfigureEmailSettings(Dictionary<string, string> allOptions)
        {
            var settings = new EmailSettings
            {
                Password = GetValueIfExists(SettingKeys.EmailSmtpPassword, allOptions),
                SendFromEmail = GetValueIfExists(SettingKeys.EmailSendFromEmail, allOptions),
                SendFromName = GetValueIfExists(SettingKeys.EmailSendFromName, allOptions),
                SmtpHost = GetValueIfExists(SettingKeys.EmailSmtpServer, allOptions),
                Username = GetValueIfExists(SettingKeys.EmailSmtpUsername, allOptions)
            };

            string portAsString = GetValueIfExists(SettingKeys.EmailSmtpPort, allOptions);
            if (!string.IsNullOrWhiteSpace(portAsString) && int.TryParse(portAsString, out int port))
            {
                settings.SmtpPort = port;
            }
            
            // at the end assign
            EmailSettings = settings;
        }

        private void ConfigureSocial(Dictionary<string, string> allOptions)
        {
            SocialLinks = new SocialLinks
            {
                Facebook = GetValueIfExists(SettingKeys.SocialFacebook, allOptions),
                Instagram = GetValueIfExists(SettingKeys.SocialInstagram, allOptions),
                Twitter = GetValueIfExists(SettingKeys.SocialTwitter, allOptions)
            };
        }

        private string GetValueIfExists(string key, Dictionary<string, string> allOptions)
        {
            var item = allOptions.FirstOrDefault(o => o.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(item.Value))
            {
                return null;
            }

            return item.Value;
        }

        #endregion

        #region Static Methods

        public static void Reset() // is this ok?
        {
            _Lazy = CreateLazy();
        }

        private static Lazy<AppConfig> CreateLazy()
        {
            return new Lazy<AppConfig>(() => new AppConfig(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        public static void Configure(Func<IConfigService> dbContextFactory)
        {
            _DbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public TimeSpan GetLoginDuration()
        {
            return TimeSpan.FromDays(7); // for now
        }

        #endregion
    }
}