using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using Dmc.Utilities;

namespace Dmc.Cms.App.Services
{
    public class ConfigService : ServiceBase, IConfigService
    {
        public ConfigService(ICmsUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Task<ReCaptchaSettings> GetRecaptchaSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> SaveAsync(ReCaptchaSettings socialLinks)
        {
            throw new NotImplementedException();
        }

        public ExternalLoginSettings GetExternalLoginSettings()
        {
            var allOptions = GetAllOptions();
            return CreateExternalLoginSettingsFromOptions(allOptions);
        }

        public async Task<ExternalLoginSettings> GetExternalLoginSettingsAsync()
        {
            var allOptions = await GetAllOptionsAsync();
            return CreateExternalLoginSettingsFromOptions(allOptions);
        }

        private ExternalLoginSettings CreateExternalLoginSettingsFromOptions(IEnumerable<Option> allOptions)
        {
            List<ExternalLogin> result = new List<ExternalLogin>();

            foreach (string identifier in ExternalLoginSettings.SupportedExternalLoginIds)
            {
                ExternalLogin login = TryCreateExternalLogin(identifier, allOptions);

                if (login != null)
                {
                    result.Add(login);
                }
            }

            return new ExternalLoginSettings(result);
        }

        private ExternalLogin TryCreateExternalLogin(string identifier, IEnumerable<Option> allOptions)
        {
            string providerId = GetValueIfExists(string.Format(SettingKeys.ExternalLoginProviderClientIdFormat, identifier), allOptions);
            string name = GetValueIfExists(string.Format(SettingKeys.ExternalLoginProviderNameFormat, identifier), allOptions);

            if (!string.IsNullOrWhiteSpace(providerId) && !string.IsNullOrWhiteSpace(name))
            {
                return new ExternalLogin(identifier, providerId, name);
            }

            return null;
        }

        public async Task<EmailSettings> GetEmailSettingsAsync()
        {
            var allOptions = await GetAllOptionsAsync(); // we need to have a possibility to load only some
            return new EmailSettings
            {
                Password = GetValueIfExists(SettingKeys.EmailSmtpPassword, allOptions),
                SendFromEmail = GetValueIfExists(SettingKeys.EmailSendFromEmail, allOptions),
                SendFromName = GetValueIfExists(SettingKeys.EmailSendFromName, allOptions),
                SmtpHost = GetValueIfExists(SettingKeys.EmailSmtpServer, allOptions),
                SmtpPort = DmcConvert.ToInt32(GetValueIfExists(SettingKeys.EmailSmtpPort, allOptions)),
                Username = GetValueIfExists(SettingKeys.EmailSmtpUsername, allOptions)
            };
        }

        public async Task<SiteSettings> GetSiteSettingsAsync()
        {
            var allOptions = await GetAllOptionsAsync();

            return new SiteSettings
            {
                Description = GetValueIfExists(SettingKeys.SiteSettingsDescription, allOptions),
                LatestUserFavouritePosts = DmcConvert.ToInt32(GetValueIfExists(SettingKeys.SiteSettingsLatestUserFavouritePosts, allOptions), 3),
                MainContactEmail = GetValueIfExists(SettingKeys.SiteSettingsMainContactEmail, allOptions),
                Name = GetValueIfExists(SettingKeys.SiteSettingsName, allOptions),
                NumberOfRecentPostsToShow = DmcConvert.ToInt32(GetValueIfExists(SettingKeys.SiteSettingsNumberOfRecentPosts, allOptions), 5), 
                PostsPerPage = DmcConvert.ToInt32(GetValueIfExists(SettingKeys.SiteSettingPostsPerPage, allOptions), 5),

                AnalyticsCode = GetValueIfExists(SettingKeys.SiteSettingsAnalyticsCode, allOptions),
                CustomCookieSolutionCode = GetValueIfExists(SettingKeys.SiteSettingsCustomCookieSolutionCode, allOptions),
                AdditionalCustomHeadContent = GetValueIfExists(SettingKeys.SiteSettingsAdditionalCustomHeadContent, allOptions),
                CustomCookieAndPrivacyPolicyLinks = GetValueIfExists(SettingKeys.SiteSettingsCustomCookieAndPrivacyPolicyLinks, allOptions),

                IsCustomCookieSolutionEnabled = DmcConvert.ToBool(GetValueIfExists(SettingKeys.SiteSettingsIsCustomCookieSolutionEnabled, allOptions), false),
                IsAnalyticsEnabled = DmcConvert.ToBool(GetValueIfExists(SettingKeys.SiteSettingsIsAnalyticsEnabled, allOptions), false)
            };
        }

        public async Task<ContentSettings> GetContentSettingsAsync()
        {
            var allOptions = await GetAllOptionsAsync();

            return new ContentSettings
            {
                IsAdvertisingEnabled = DmcConvert.ToBool(GetValueIfExists(SettingKeys.ContentSettingsIsAdvertisingEnabled, allOptions), false),
                IsShowingEventsEnabled = DmcConvert.ToBool(GetValueIfExists(SettingKeys.ContentSettingsIsShowingEventsEnabled, allOptions), false)
            };
        }

        public async Task<SocialLinks> GetSocialLinksAsync()
        {
            var allOptions = await GetAllOptionsAsync();
            return new SocialLinks
            {
                Facebook = GetValueIfExists(SettingKeys.SocialFacebook, allOptions),
                Instagram = GetValueIfExists(SettingKeys.SocialInstagram, allOptions),
                Twitter = GetValueIfExists(SettingKeys.SocialTwitter, allOptions)
            };
        }

        public async Task<SiteResources> GetSiteResourcesAsync()
        {
            var allOptions = await GetAllOptionsAsync();
            return new SiteResources
            {
                DefaultOpenGraphImage = GetValueIfExists(SettingKeys.Resources_DefaultOpenGraphImage, allOptions),
                DefaultSiteHeader = GetValueIfExists(SettingKeys.Resources_DefaultSiteHeader, allOptions),
                EmailHeader = GetValueIfExists(SettingKeys.Resources_EmailHeader, allOptions),
                FooterLogo = GetValueIfExists(SettingKeys.Resources_FooterLogo, allOptions),
                Logo = GetValueIfExists(SettingKeys.Resources_Logo, allOptions),
                RetinaLogo = GetValueIfExists(SettingKeys.Resources_RetinaLogo, allOptions),
            };
        }

        public async Task<ServiceResult> SaveAsync(SiteResources model)
        {
            var allOptions = await GetAllOptionsAsync();

            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_DefaultOpenGraphImage, model.DefaultOpenGraphImage, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_DefaultSiteHeader, model.DefaultSiteHeader, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_EmailHeader, model.EmailHeader, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_FooterLogo, model.FooterLogo, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_Logo, model.Logo, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.Resources_RetinaLogo, model.RetinaLogo, allOptions);

            return await SaveAsync();
        }

        public async Task<ServiceResult> SaveAsync(ContentSettings emailSettings)
        {
            var allOptions = await GetAllOptionsAsync();

            UpdateOrCreateNewAndSetToRepository(SettingKeys.ContentSettingsIsAdvertisingEnabled, emailSettings.IsAdvertisingEnabled.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.ContentSettingsIsShowingEventsEnabled, emailSettings.IsShowingEventsEnabled.ToString(), allOptions);

            return await SaveAsync();
        }

        public async Task<ServiceResult> SaveAsync(SiteSettings siteSettings)
        {
            var allOptions = await GetAllOptionsAsync();

            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingPostsPerPage, siteSettings.PostsPerPage.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsLatestUserFavouritePosts, siteSettings.LatestUserFavouritePosts.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsNumberOfRecentPosts, siteSettings.NumberOfRecentPostsToShow.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsName, siteSettings.Name, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsDescription, siteSettings.Description, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsMainContactEmail, siteSettings.MainContactEmail, allOptions);

            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsAnalyticsCode, siteSettings.AnalyticsCode, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsIsAnalyticsEnabled, siteSettings.IsAnalyticsEnabled.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsCustomCookieSolutionCode, siteSettings.CustomCookieSolutionCode, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsIsCustomCookieSolutionEnabled, siteSettings.IsCustomCookieSolutionEnabled.ToString(), allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsAdditionalCustomHeadContent, siteSettings.AdditionalCustomHeadContent, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SiteSettingsCustomCookieAndPrivacyPolicyLinks, siteSettings.CustomCookieAndPrivacyPolicyLinks, allOptions);

            return await SaveAsync();
        }

        public async Task<ServiceResult> SaveAsync(EmailSettings emailSettings)
        {
            var allOptions = await GetAllOptionsAsync();

            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSendFromEmail, emailSettings.SendFromEmail ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSendFromName, emailSettings.SendFromName ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSmtpPassword, emailSettings.Password ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSmtpPort, emailSettings.SmtpPort.HasValue ? emailSettings.SmtpPort.Value.ToString() : string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSmtpServer, emailSettings.SmtpHost ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.EmailSmtpUsername, emailSettings.Username ?? string.Empty, allOptions);

            return await SaveAsync();
        }

        public async Task<ServiceResult> SaveAsync(IEnumerable<ExternalLogin> result)
        {
            var allOptions = await GetAllOptionsAsync();

            foreach (var item in result)
            {
                TryUpdateOrCreateNewSettingIfExternalLoginValid(item, allOptions);
            }

            return await SaveAsync();
        }

        private void TryUpdateOrCreateNewSettingIfExternalLoginValid(ExternalLogin item, IEnumerable<Option> allOptions)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ServiceIdentifier))
            {
                return;
            }

            if (!ExternalLoginSettings.SupportedExternalLoginIds.Any(o => item.ServiceIdentifier.Equals(item.ServiceIdentifier, StringComparison.Ordinal)))
            {
                return; // Unsupported
            }

            if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.ClientId))
            {
                return;
            }

            // should throw. this is incorrect use of service.
            //string providerId = GetValueIfExists(string.Format(SettingKeys.ExternalLoginProviderClientIdFormat, identifier), allOptions);
            //string name = GetValueIfExists(string.Format(SettingKeys.ExternalLoginProviderNameFormat, identifier), allOptions);

            UpdateOrCreateNewAndSetToRepository(string.Format(SettingKeys.ExternalLoginProviderClientIdFormat, item.ServiceIdentifier), item.ClientId ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(string.Format(SettingKeys.ExternalLoginProviderNameFormat, item.ServiceIdentifier), item.Name ?? string.Empty, allOptions);
        }

        public async Task<ServiceResult> SaveAsync(SocialLinks socialLinks)
        {
            var allOptions = await GetAllOptionsAsync();

            UpdateOrCreateNewAndSetToRepository(SettingKeys.SocialFacebook, socialLinks.Facebook ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SocialInstagram, socialLinks.Instagram ?? string.Empty, allOptions);
            UpdateOrCreateNewAndSetToRepository(SettingKeys.SocialTwitter, socialLinks.Twitter ?? string.Empty, allOptions);

            return await SaveAsync();
        }

        public Task<IEnumerable<Option>> GetAllOptionsAsync()
        {
            return UnitOfWork.OptionRepository.GetOptionsAsync();
        }

        public Task<Option> GetOptionByNameAsync(string name)
        {
            return UnitOfWork.OptionRepository.GetOptionByNameAsync(name);
        }

        public Task<ServiceResult> InsertAsync(Option entity)
        {
            UnitOfWork.OptionRepository.Insert(entity);
            return SaveAsync();
        }

        public Task<ServiceResult> UpdateAsync(Option entity)
        {
            UnitOfWork.OptionRepository.Update(entity);
            return SaveAsync();
        }

        private Option UpdateOrCreateNewAndSetToRepository(string key, string value, IEnumerable<Option> allOptions)
        {
            Option item = allOptions.FirstOrDefault(o => o.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            bool isNew = item == null;

            if (isNew)
            {
                item = new Option
                {
                    Name = key
                };
            }

            item.Value = value;
            item.Modified = DateTimeOffset.UtcNow;

            if (isNew)
            {
                UnitOfWork.OptionRepository.Insert(item);
            }
            else
            {
                UnitOfWork.OptionRepository.Update(item); // nothing here as we should have it in memory
            }

            return item;
        }

        private string GetValueIfExists(string key, IEnumerable<Option> allOptions)
        {
            var item = allOptions.FirstOrDefault(o => o.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                return null;
            }

            return item.Value;
        }

        public IEnumerable<Option> GetAllOptions()
        {
            return UnitOfWork.OptionRepository.GetOptions();
        }

        
    }
}
