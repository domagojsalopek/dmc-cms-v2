using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dmc.Cms.App
{
    public class SiteSettings : ISiteSettings
    {
        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string MainContactEmail
        {
            get;
            set;
        }

        public bool IsAnalyticsEnabled
        {
            get;
            set;
        }

        [AllowHtml] // this is shit. we need to use viewmodel
        public string AnalyticsCode
        {
            get;
            set;
        }

        public bool IsCustomCookieSolutionEnabled
        {
            get;
            set;
        }

        [AllowHtml]
        public string CustomCookieSolutionCode
        {
            get;
            set;
        }

        [AllowHtml]
        public string AdditionalCustomHeadContent
        {
            get;
            set;
        }

        [AllowHtml]
        public string CustomCookieAndPrivacyPolicyLinks
        {
            get;
            set;
        }

        //TODO: Move these settings below to content settings

        public int PostsPerPage
        {
            get;
            set;
        }

        public int LatestUserFavouritePosts
        {
            get;
            set;
        }

        public int NumberOfRecentPostsToShow
        {
            get;
            set;
        }
    }
}
