using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public interface ISitemapService : IService
    {
        string CategoriesBaseUrl { get; set; }
        string EventsBaseUrl { get; set; }
        string PostsBaseUrl { get; set; }

        Task<bool> TryGenerateAndSaveAsync(string sitemapLocation);
    }
}