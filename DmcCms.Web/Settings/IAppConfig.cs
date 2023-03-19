using System;
using Dmc.Cms.App;

namespace Dmc.Cms.Web.Settings
{
    public interface IAppConfig
    {
        IContentSettings ContentSettings { get; }

        ISocialLinks SocialLinks { get; }

        IEmailSettings EmailSettings { get; }

        ISiteSettings SiteSettings { get; }

        ExternalLoginSettings ExternalLoginSettings { get; }

        ISiteResources SiteResources { get; }

        IReCaptchaSettings ReCaptchaSettings { get; }

        TimeSpan GetLoginDuration();
    }
}