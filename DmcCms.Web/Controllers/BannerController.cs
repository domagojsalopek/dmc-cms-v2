using Dmc.Cms.App;
using Dmc.Cms.Model;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class BannerController : Controller
    {
        private const int CacheDurationInMinutes = 60 * 6;
        private readonly IAdvertisementService _Adservice;
        private static readonly Random _Random = new Random();

        public BannerController(IAdvertisementService service)
        {
            _Adservice = service;
        }

        public async Task<PartialViewResult> Random(AdvertisementType? type)
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            AdvertisementType advertisementType = type ?? AdvertisementType.Aside;

            string cacheKey = CreateCacheKey(advertisementType);
            List<AdvertisementViewModel> resultsFromCache = CacheHelper.GetFromCache<List<AdvertisementViewModel>>(cacheKey);

            if (resultsFromCache != null)
            {
                if (advertisementType == AdvertisementType.BelowContent)
                {
                    return PartialView("Partials/_BelowContentRandomSingleAdPartial", GetRandomItemFromList(resultsFromCache));
                }

                return PartialView("Partials/_RandomSingleAdPartial", GetRandomItemFromList(resultsFromCache));
            }

            var ads = await _Adservice.GetAllVisibleOfTypeAsync(type ?? AdvertisementType.Aside);

            if (ads == null || ads.Count() <= 0)
            {
                return PartialView("Partials/_NoAdsPartial");
            }

            List<AdvertisementViewModel> results = new List<AdvertisementViewModel>();

            foreach (Advertisement ad in ads)
            {
                results.Add(new AdvertisementViewModel
                {
                    Html = ad.Html,
                    UniqueId = ad.UniqueId
                });
            }

            // add to cache so we have it now. cache is reset when ad is saved or updated.
            CacheHelper.AddToCache(cacheKey, results, DateTimeOffset.UtcNow.AddMinutes(CacheDurationInMinutes));

            // return view with on randomly selected.
            if (advertisementType == AdvertisementType.BelowContent)
            {
                return PartialView("Partials/_BelowContentRandomSingleAdPartial", GetRandomItemFromList(results));
            }
            
            return PartialView("Partials/_RandomSingleAdPartial", GetRandomItemFromList(results));
        }

        private AdvertisementViewModel GetRandomItemFromList(List<AdvertisementViewModel> resultsFromCache)
        {
            int randomValue = _Random.Next(resultsFromCache.Count);
            return resultsFromCache[randomValue];
        }

        private static string CreateCacheKey(AdvertisementType advertisementType)
        {
            return string.Concat("Advertisement_AllVisibleAds_", advertisementType.ToString());
        }
    }
}