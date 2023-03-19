namespace Dmc.Cms.App
{
    public interface ISiteSettings
    {
        string Description
        {
            get;
            set;
        }

        int LatestUserFavouritePosts
        {
            get;
            set;
        }

        string CustomCookieAndPrivacyPolicyLinks
        {
            get;
            set;
        }

        string MainContactEmail
        {
            get;
            set;
        }

        string Name
        {
            get;
            set;
        }

        int NumberOfRecentPostsToShow
        {
            get;
            set;
        }

        int PostsPerPage
        {
            get;
            set;
        }

        bool IsAnalyticsEnabled
        {
            get;
            set;
        }

        string AnalyticsCode
        {
            get;
            set;
        }

        bool IsCustomCookieSolutionEnabled
        {
            get;
            set;
        }

        string CustomCookieSolutionCode
        {
            get;
            set;
        }

        string AdditionalCustomHeadContent
        {
            get;
            set;
        }
    }
}