using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Cms.App
{
    public static class CacheHelper
    {
        #region Fields

        private static readonly HashSet<string> _CacheKeysUsed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Add To Cache

        public static void AddToCache(string cacheKey, object value)
        {
            AddToCache(cacheKey, value, ObjectCache.InfiniteAbsoluteExpiration);
        }

        public static void AddToCache(string cacheKey, object value, DateTimeOffset expires)
        {
            if (MemoryCache.Default.Add(cacheKey, value, expires))
            {
                _CacheKeysUsed.Add(cacheKey);
            }
        }

        #endregion

        #region Remove From Cache

        public static void RemoveFromCache(string cacheKey)
        {
            MemoryCache.Default.Remove(cacheKey);
        }

        public static void ResetEntireCache()
        {
            foreach (string item in _CacheKeysUsed)
            {
                RemoveFromCache(item);
            }

            _CacheKeysUsed.Clear();
        }

        #endregion

        #region Get From Cache

        public static T GetFromCache<T>(string cacheKey) where T : class
        {
            return MemoryCache.Default.Get(cacheKey) as T;
        }

        #endregion
    }
}
