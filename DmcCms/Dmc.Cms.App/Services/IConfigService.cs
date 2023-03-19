using Dmc.Cms.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface IConfigService : IService, IDisposable
    {
        ExternalLoginSettings GetExternalLoginSettings();

        Task<ExternalLoginSettings> GetExternalLoginSettingsAsync();

        Task<ServiceResult> SaveAsync(IEnumerable<ExternalLogin> result);

        IEnumerable<Option> GetAllOptions();
        Task<IEnumerable<Option>> GetAllOptionsAsync();
        Task<Option> GetOptionByNameAsync(string name);
        Task<ServiceResult> InsertAsync(Option entity);
        Task<ServiceResult> UpdateAsync(Option entity);

        Task<ReCaptchaSettings> GetRecaptchaSettingsAsync();
        Task<ServiceResult> SaveAsync(ReCaptchaSettings socialLinks);

        Task<SocialLinks> GetSocialLinksAsync();
        Task<ServiceResult> SaveAsync(SocialLinks socialLinks);

        Task<EmailSettings> GetEmailSettingsAsync();
        Task<SiteSettings> GetSiteSettingsAsync();

        Task<ContentSettings> GetContentSettingsAsync();

        Task<ServiceResult> SaveAsync(ContentSettings emailSettings);

        Task<ServiceResult> SaveAsync(SiteSettings emailSettings);
        Task<ServiceResult> SaveAsync(EmailSettings emailSettings);
        Task<SiteResources> GetSiteResourcesAsync();
        Task<ServiceResult> SaveAsync(SiteResources model);
    }
}
