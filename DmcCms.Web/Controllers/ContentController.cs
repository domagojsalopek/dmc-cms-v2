using Dmc.Cms.App;
using Dmc.Cms.App.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class ContentController : Controller
    {
        #region Private Fields

        private readonly IImageService _ImageService;
        private const string LogoResourceUrl = "~/resources/custom/site-logo2x.png";
        private const string LogoImageType = "image/png";
        private const int CacheDurationInMinutes = 180;

        // TODO: much, much better
        private static readonly int[] _AllowedDimensions = new int[]
        {
            48,
            96,
            192,
            384
        };

        #endregion

        #region Constructors

        public ContentController(IImageService service)
        {
            _ImageService = service ?? throw new ArgumentNullException(nameof(service));
        }

        #endregion

        #region Preview Images

        [OutputCache(Duration = 31536000, VaryByParam = "*", NoStore = false)]
        public async Task<ActionResult> Image(int? imageId, int? width, int? height, bool large = false)
        {
            if (!imageId.HasValue || !width.HasValue || !height.HasValue)
            {
                return File(LogoResourceUrl, LogoImageType);
            }

            string cacheKey = CreateCacheKey(imageId.Value, width.Value, height.Value, large);
            ImageInfo imageInfo = HttpRuntime.Cache.Get(cacheKey) as ImageInfo;

            if (imageInfo != null)
            {
                return File(imageInfo.FullPath, imageInfo.FileType);
            }

            if (!DimensionsAllowed(width.Value, height.Value))
            {
                return File(LogoResourceUrl, LogoImageType);
            }

            var imageObject = await _ImageService.GetByIdAsync(imageId.Value);

            if (imageObject == null)
            {
                return File(LogoResourceUrl, LogoImageType);
            }

            string largeOrSmall = large 
                ? imageObject.LargeImage 
                : imageObject.SmallImage;

            FileInfo fileInfo = new FileInfo(Server.MapPath(largeOrSmall));
            imageInfo = ImageResizer.MapAndResizeImageIfNeeded(fileInfo.DirectoryName, fileInfo.Name, width.Value, height.Value);

            if (imageInfo == null)
            {
                return File(LogoResourceUrl, LogoImageType);
            }

            HttpRuntime.Cache.Add(cacheKey
                    , imageInfo
                    , null
                    , DateTime.UtcNow.AddMinutes(CacheDurationInMinutes)
                    , System.Web.Caching.Cache.NoSlidingExpiration
                    , System.Web.Caching.CacheItemPriority.Low
                    , null);

            return File(imageInfo.FullPath, imageInfo.FileType);
        }

        #endregion

        #region Private Methods

        private static string CreateCacheKey(int postId, int width, int height, bool large)
        {
            return string.Concat("Image_", postId, width, height, large);
        }

        private bool DimensionsAllowed(int widnt, int height)
        {
            return _AllowedDimensions.Any(o => o == widnt) && 
                   _AllowedDimensions.Any(o => o == height);
        }

        #endregion
    }
}