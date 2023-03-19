using Dmc.Cms.App;
using Dmc.Cms.App.Services;
using Dmc.Cms.Web.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class RssController : Controller
    {
        #region Constants

        private const int HowLongToKeepFeedInCacheInMinutes = 120;
        private const string BlogRssCacheKey = "BlogSyndicationFeed";

        #endregion

        #region Fields

        private readonly IPostService _PostService;
        private readonly IAppConfig _AppConfig;
        private bool disposed;

        #endregion

        #region Constructor

        public RssController(IPostService service, IAppConfig appConfig)
        {
            _PostService = service ?? throw new ArgumentNullException(nameof(service));
            _AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
        }

        #endregion

        #region Index

        // GET: Rss
        public async Task<ActionResult> Index()
        {
            SyndicationFeed feed = CacheHelper.GetFromCache<SyndicationFeed>(BlogRssCacheKey); // try to get from cache

            if (feed != null)
            {
                return new FeedResult
                {
                    Feed = feed
                };
            }

            feed = new SyndicationFeed(_AppConfig.SiteSettings.Name
                , _AppConfig.SiteSettings.Name
                , new Uri(CmsUtilities.AbsoluteAction(new UrlHelper(Request.RequestContext), "Index", "Rss", new object()).ToString())
            );

            var articles = await _PostService.GetPagedPublishedAsync(1, _AppConfig.SiteSettings.PostsPerPage);
            if (articles == null || !articles.Any())
            {
                return new FeedResult() { Feed = feed };
            }

            // build feed
            var lastArticle = articles.FirstOrDefault();

            feed.Id = BlogRssCacheKey;
            feed.LastUpdatedTime = lastArticle.Published.GetValueOrDefault(lastArticle.Created);

            List<SyndicationItem> postItems = new List<SyndicationItem>();

            foreach (var item in articles)
            {
                SyndicationItem post = CreateSyndicationItem(item);

                if (post != null)
                {
                    postItems.Add(post);
                }
            }

            // set to feed
            feed.Items = postItems;

            // add to cache and return result
            CacheHelper.AddToCache(BlogRssCacheKey, feed, DateTimeOffset.UtcNow.AddHours(HowLongToKeepFeedInCacheInMinutes));

            return new FeedResult() { Feed = feed };
        }

        private SyndicationItem CreateSyndicationItem(Model.Post item)
        {
            Uri url = new Uri(CmsUtilities.AbsoluteAction(new UrlHelper(Request.RequestContext), "details", "default", new { slug = item.Slug }));
            return new SyndicationItem(item.Title
                , item.Description
                , url
                , url.AbsoluteUri
                , item.Published.GetValueOrDefault(item.Created)
            );
        }

        #endregion

        #region Overrides

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }                

            if (disposing)
            {
                if (_PostService != null)
                {
                    _PostService.Dispose();
                }
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }

        #endregion
    }
}