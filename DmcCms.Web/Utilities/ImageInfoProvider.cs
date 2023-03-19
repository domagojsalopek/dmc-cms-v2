using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Dmc.Cms.Web
{
    public static class ImageInfoProvider
    {
        private static readonly List<string> _TrackedCacheKeys = new List<string>();

        public static void ClearCache()
        {
            foreach (var item in _TrackedCacheKeys)
            {
                HttpRuntime.Cache.Remove(item);
            }
        }

        // TODO: Warmup ... read dimensions without loading entire file ... 
        // Image info should not contain full path.

        public static ImageInfo GetImageInfo(string relativePath)
        {
            string cacheKey = $"ImageInfoProvider_{relativePath}";
            ImageInfo cached = HttpRuntime.Cache.Get(cacheKey) as ImageInfo;

            if (cached != null)
            {
                return cached;
            }

            ImageInfo result = null;
            Image image = null;

            try
            {
                string fullPath = HostingEnvironment.MapPath(relativePath);
                image = Image.FromFile(fullPath);
                result = new ImageInfo
                {
                    FileType = GetFileType(image.RawFormat),
                    Height = image.Height,
                    Width = image.Width
                };
            }
            catch (Exception) // nothing for now. todo. log
            {
                return null;
            }
            finally
            {
                if (image != null)
                {
                    image.Dispose();
                }
            }

            if (result != null)
            {
                AddResultToCache(cacheKey, result);
            }

            return result;
        }

        private static string GetFileType(ImageFormat rawFormat)
        {
            if (ImageFormat.Jpeg.Equals(rawFormat))
            {
                return "image/jpeg";
            }
            else if (ImageFormat.Png.Equals(rawFormat))
            {
                return "image/png";
            }
            else if (ImageFormat.Gif.Equals(rawFormat))
            {
                return "image/gif";
            }

            return null;
        }

        private static void AddResultToCache(string cacheKey, ImageInfo result)
        {
            _TrackedCacheKeys.Add(cacheKey);
            HttpRuntime.Cache.Add(cacheKey
                                , result
                                , null
                                , System.Web.Caching.Cache.NoAbsoluteExpiration
                                , System.Web.Caching.Cache.NoSlidingExpiration
                                , System.Web.Caching.CacheItemPriority.Low
                                , null);
        }
    }
}