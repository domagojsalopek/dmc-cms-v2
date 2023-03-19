using Dmc.Cms.App;
using Dmc.Cms.Web.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web
{
    public static class CmsUtilities
    {
        const string CookieConsentScriptCookieName = "cookieconsent_status";
        public const string CMSCookieConsentCookieName = "dmc_cms_cookie_consent_status";

        public static IAppConfig GetAppConfig(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewData[AppConfig.ViewDataSettingsName] as IAppConfig;
        }

        public static string GetFbId(this HtmlHelper htmlHelper)
        {
            return GetExternalLoginClientId(ExternalLoginKeys.Facebook, htmlHelper);
        }

        public static string GetAmazonId(this HtmlHelper htmlHelper)
        {
            return GetExternalLoginClientId(ExternalLoginKeys.Amazon, htmlHelper);
        }

        private static string GetExternalLoginClientId(string key, HtmlHelper htmlHelper)
        {
            var config = GetAppConfig(htmlHelper);

            if (config == null)
            {
                return null;
            }

            var provider = config.ExternalLoginSettings?.GetExternalLogin(key);

            if (provider == null)
            {
                return null;
            }

            return provider.ClientId;
        }

        public static Uri GetBaseUrl(this UrlHelper url)
        {
            Uri contextUri = new Uri(url.RequestContext.HttpContext.Request.Url, url.RequestContext.HttpContext.Request.RawUrl);
            UriBuilder realmUri = new UriBuilder(contextUri) { Path = url.RequestContext.HttpContext.Request.ApplicationPath, Query = null, Fragment = null };
            return realmUri.Uri;
        }

        public static string AbsoluteAction(this UrlHelper url, string actionName, string controllerName, object routeValues)
        {
            return new Uri(GetBaseUrl(url), url.Action(actionName, controllerName, routeValues)).AbsoluteUri;
        }

        public static string AbsoluteContent(this UrlHelper url, string content)
        {
            var baseURI = new Uri(GetBaseUrl(url), content.TrimStart('~'));
            return new Uri(baseURI.AbsoluteUri, UriKind.Absolute).AbsoluteUri;
        }

        public static bool IsCookieConsentGiven(this HttpRequestBase htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            if (htmlHelper.Cookies[CMSCookieConsentCookieName] != null)
            {
                return true; // our cookie only needs to be present
            }

            return IsCookieScriptCookiePresentAndAccepted(htmlHelper);
        }

        public static bool IsCookieConsentGiven(this HttpContextBase htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            return IsCookieConsentGiven(htmlHelper.Request);
        }

        public static bool IsCookieConsentGiven(this HtmlHelper htmlHelper)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException(nameof(htmlHelper));
            }

            return IsCookieConsentGiven(htmlHelper.ViewContext.RequestContext.HttpContext.Request);
        }

        private static bool IsCookieScriptCookiePresentAndAccepted(HttpRequestBase htmlHelper)
        {
            var cookie = htmlHelper.Cookies[CookieConsentScriptCookieName];

            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
            {
                return false;
            }

            return cookie.Value.Equals("dismiss", StringComparison.OrdinalIgnoreCase) || cookie.Value.Equals("allow", StringComparison.OrdinalIgnoreCase);
        }
    }
}