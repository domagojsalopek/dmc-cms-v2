using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Web.Attributes;
using Dmc.Cms.Web.Settings;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleKeys.Admin)]
    [NoCache]
    public class ConfigController : ControllerBase
    {
        private const string UploadDirectory = "~/uploads/resources";

        public ConfigController(ApplicationUserManager applicationUserManager, IConfigService configService) 
            : base(applicationUserManager)
        {
            ConfigService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        public IConfigService ConfigService { get; }

        //public ActionResult Index()
        //{
        //    return View();
        //}

        #region Social Links

        public async Task<ActionResult> SocialLinks()
        {
            SocialLinks links = await ConfigService.GetSocialLinksAsync();
            return View(links);
        }

        [HttpPost]
        public async Task<ActionResult> SocialLinks(SocialLinks socialLinks)
        {
            var result = await ConfigService.SaveAsync(socialLinks);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(SocialLinks));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(socialLinks);
        }

        #endregion

        #region ReCaptcha

        public async Task<ActionResult> ReCaptcha()
        {
            ReCaptchaSettings links = await ConfigService.GetRecaptchaSettingsAsync();
            return View(links);
        }

        [HttpPost]
        public async Task<ActionResult> ReCaptcha(ReCaptchaSettings socialLinks)
        {
            var result = await ConfigService.SaveAsync(socialLinks);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(ReCaptcha));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(socialLinks);
        }

        #endregion

        #region Social Links

        public async Task<ActionResult> ExternalLoginSettings()
        {
            ExternalLoginSettings links = await ConfigService.GetExternalLoginSettingsAsync();

            if (links == null)
            {
                return Error();
            }

            AdminExternalLoginSettingsViewModel settingsViewModel = new AdminExternalLoginSettingsViewModel();

            foreach (string item in Dmc.Cms.App.ExternalLoginSettings.SupportedExternalLoginIds)
            {
                settingsViewModel.Settings.Add(CreateSettingViewModel(item, links));
            }

            return View(settingsViewModel);
        }

        private AdminExternalLoginSettingViewModel CreateSettingViewModel(string item, ExternalLoginSettings links)
        {
            var info = links.GetExternalLogin(item);
            return new AdminExternalLoginSettingViewModel
            {
                ClientId = info?.ClientId,
                Name = info?.Name,
                ServiceIdentifier = item
            };
        }

        [HttpPost]
        public async Task<ActionResult> ExternalLoginSettings(AdminExternalLoginSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            List<ExternalLogin> settings = CreateSettingsFromViewModel(model);

            if (settings == null)
            {
                return null;
            }

            var result = await ConfigService.SaveAsync(settings);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(ExternalLoginSettings));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(model);
        }

        private List<ExternalLogin> CreateSettingsFromViewModel(AdminExternalLoginSettingsViewModel socialLinks)
        {
            List<ExternalLogin> result = new List<ExternalLogin>();

            foreach (var item in socialLinks.Settings)
            {
                result.Add(new ExternalLogin(item.ServiceIdentifier, item.ClientId, item.Name));
            }

            return result;
        }

        #endregion

        #region Content Settings

        public async Task<ActionResult> ContentSettings()
        {
            ContentSettings links = await ConfigService.GetContentSettingsAsync();
            return View(links);
        }

        [HttpPost]
        public async Task<ActionResult> ContentSettings(ContentSettings socialLinks)
        {
            var result = await ConfigService.SaveAsync(socialLinks);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(ContentSettings));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(socialLinks);
        }

        #endregion

        #region Site Resources

        public async Task<ActionResult> SiteResources()
        {
            SiteResources links = await ConfigService.GetSiteResourcesAsync();
            AdminSiteResourcesViewModel viewModel = new AdminSiteResourcesViewModel
            {
                DefaultOpenGraphImage = links.DefaultOpenGraphImage,
                DefaultSiteHeader = links.DefaultSiteHeader,
                EmailHeader = links.EmailHeader,
                FooterLogo = links.FooterLogo,
                Logo = links.Logo,
                RetinaLogo = links.RetinaLogo
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SiteResources(AdminSiteResourcesViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            SiteResources model = CreateFromViewModel(viewModel);
            ServiceResult result = await ConfigService.SaveAsync(model);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();
                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(SiteResources));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(viewModel);
        }

        private SiteResources CreateFromViewModel(AdminSiteResourcesViewModel socialLinks)
        {
            SiteResources siteResources = new SiteResources();

            SetImagePathsToModel(socialLinks, siteResources);

            return siteResources;
        }

        private void SetImagePathsToModel(AdminSiteResourcesViewModel socialLinks, SiteResources siteResources)
        {
            string rootPath = Server.MapPath(UploadDirectory);
            DirectoryInfo uploadsFolder = new DirectoryInfo(rootPath);

            if (!uploadsFolder.Exists)
            {
                uploadsFolder.Create();
            }

            siteResources.DefaultOpenGraphImage = UploadImageAndGetPathIfNeeded(socialLinks.DefaultOpenGraphImageUpload, uploadsFolder) ?? siteResources.DefaultOpenGraphImage;
            siteResources.DefaultSiteHeader = UploadImageAndGetPathIfNeeded(socialLinks.DefaultSiteHeaderUpload, uploadsFolder) ?? siteResources.DefaultSiteHeader;
            siteResources.EmailHeader = UploadImageAndGetPathIfNeeded(socialLinks.EmailHeaderUpload, uploadsFolder) ?? siteResources.EmailHeader;
            siteResources.FooterLogo = UploadImageAndGetPathIfNeeded(socialLinks.FooterLogoUpload, uploadsFolder) ?? siteResources.FooterLogo;
            siteResources.Logo = UploadImageAndGetPathIfNeeded(socialLinks.LogoUpload, uploadsFolder) ?? siteResources.Logo;
            siteResources.RetinaLogo = UploadImageAndGetPathIfNeeded(socialLinks.RetinaLogoUpload, uploadsFolder) ?? siteResources.RetinaLogo;
        }

        private string UploadImageAndGetPathIfNeeded(HttpPostedFileBase defaultOpenGraphImageUpload, DirectoryInfo uploadsFolder)
        {
            if (defaultOpenGraphImageUpload != null && defaultOpenGraphImageUpload.ContentLength > 0)
            {
                defaultOpenGraphImageUpload.SaveAs(Path.Combine(uploadsFolder.FullName, defaultOpenGraphImageUpload.FileName).ToLower());
                return string.Format("~{0}?v={1}"
                    , Url.Content(Path.Combine(UploadDirectory, defaultOpenGraphImageUpload.FileName).ToLower())
                    , DateTimeOffset.UtcNow.UtcTicks);
            }

            return null;
        }

        #endregion

        #region Site Settings

        public async Task<ActionResult> SiteSettings()
        {
            SiteSettings links = await ConfigService.GetSiteSettingsAsync();
            return View(links);
        }

        [HttpPost]
        public async Task<ActionResult> SiteSettings(SiteSettings socialLinks)
        {
            ServiceResult result = await ConfigService.SaveAsync(socialLinks);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(SiteSettings));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(socialLinks);
        }

        #endregion

        #region Email Settings

        public async Task<ActionResult> EmailSettings()
        {
            EmailSettings links = await ConfigService.GetEmailSettingsAsync();
            return View(links);
        }

        [HttpPost]
        public async Task<ActionResult> EmailSettings(EmailSettings socialLinks)
        {
            ServiceResult result = await ConfigService.SaveAsync(socialLinks);

            if (result.Success)
            {
                // THIS IS WRONG!
                Settings.AppConfig.Reset();

                AddMessageToTempData(MessageType.Success, "Changes Saved Successfully.");
                return RedirectToAction(nameof(EmailSettings));
            }

            AddErrorMessagesToViewData(result.Errors);
            return View(socialLinks);
        }

        #endregion
    }
}