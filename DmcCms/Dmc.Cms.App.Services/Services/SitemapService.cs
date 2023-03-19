using Dmc.Cms.Model;
using Dmc.Cms.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dmc.Cms.App.Services
{
    public class SitemapService : ISitemapService
    {
        private static readonly XNamespace _XNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";

        public SitemapService(ICmsUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public string PostsBaseUrl
        {
            get;
            set;
        }

        public string CategoriesBaseUrl
        {
            get;
            set;
        }

        public string EventsBaseUrl
        {
            get;
            set;
        }

        public ICmsUnitOfWork UnitOfWork { get; }

        // TODO: METHOD TO UPDATE SINGLE ENTRY ASAP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        public async Task<bool> TryGenerateAndSaveAsync(string sitemapLocation)
        {
            XElement root = new XElement(_XNamespace + "urlset");

            await AppendCategories(root);

            await AppendPosts(root);

            await AppendEvents(root);

            try
            {
                XDocument xDocument = new XDocument(root);
                xDocument.Save(sitemapLocation);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task AppendEvents(XElement root)
        {
            if (string.IsNullOrWhiteSpace(EventsBaseUrl))
            {
                return;
            }

            IEnumerable<Event> posts = await UnitOfWork.EventRepository.GetPagedAsync(1, int.MaxValue); // for now

            if (posts == null)
            {
                return;
            }

            foreach (Event post in posts)
            {
                if (!post.CanBeDisplayed)
                {
                    continue;
                }

                string formattedDate = string.Format("{0}-{1}"
                    , post.EventDate.Month.ToString().PadLeft(2, '0')
                    , post.EventDate.Day.ToString().PadLeft(2, '0'));

                XElement url = new XElement(_XNamespace + "url"
                    , new XElement(_XNamespace + "loc", $"{EventsBaseUrl}/{formattedDate}/{post.Slug}")
                    , new XElement(_XNamespace + "lastmod", post.Modified.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                );

                root.Add(url);
            }
        }

        private async Task AppendPosts(XElement root)
        {
            if (string.IsNullOrWhiteSpace(PostsBaseUrl))
            {
                return;
            }

            IEnumerable<Post> posts = await UnitOfWork.PostRepository.GetPagedAsync(1, int.MaxValue); // for now

            if (posts == null)
            {
                return;
            }

            foreach (Post post in posts)
            {
                if (!post.CanBeDisplayed)
                {
                    continue;
                }

                XElement url = new XElement(_XNamespace + "url"
                    , new XElement(_XNamespace + "loc", $"{PostsBaseUrl}/{post.Slug}")
                    , new XElement(_XNamespace + "lastmod", post.Modified.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                );

                root.Add(url);
            }
        }

        private async Task AppendCategories(XElement root)
        {
            if (string.IsNullOrWhiteSpace(CategoriesBaseUrl))
            {
                return;
            }

            IEnumerable<Category> categories = await UnitOfWork.CategoryRepository.GetAllCategoriesAsync();

            if (categories == null)
            {
                return;
            }

            foreach (Category category in categories)
            {
                if (!category.CanBeDisplayed)
                {
                    continue;
                }

                XElement url = new XElement(_XNamespace + "url"
                    , new XElement(_XNamespace + "loc", $"{CategoriesBaseUrl}/{category.Slug}")
                    , new XElement(_XNamespace + "lastmod", category.Modified.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                );

                root.Add(url);
            }
        }
    }
}
