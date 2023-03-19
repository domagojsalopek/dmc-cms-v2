using Dmc.Cms.App;
using Dmc.Cms.App.Identity;
using Dmc.Cms.Model;
using Dmc.Cms.Web.Mappers;
using Dmc.Cms.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dmc.Cms.Web.Controllers
{
    public class EventController : ControllerBase
    {
        #region Fields

        private const int HowLongToKeepInCacheMinutes = 60;
        private readonly IEventService _Service;

        #endregion

        #region Constructors

        public EventController(IEventService eventService, ApplicationUserManager manager) 
            : base(manager)
        {
            _Service = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        #endregion

        #region Details

        public async Task<ActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            string cacheKey = string.Format("Event_{0}", slug.ToUpper()); // normalize slug to upper to not duplicate cache for one uppercase letter for example.
            EventViewModel detailsViewModel = CacheHelper.GetFromCache<EventViewModel>(cacheKey);
            if (detailsViewModel == null)
            {
                detailsViewModel = await GetEntityAndLoadViewModelAsync(slug);
            }

            if (detailsViewModel == null)
            {
                return NotFound();
            }

            CacheHelper.AddToCache(cacheKey, detailsViewModel, DateTimeOffset.UtcNow.AddMinutes(HowLongToKeepInCacheMinutes));
            return View("Details", detailsViewModel);
        }

        #endregion

        #region List

        public async Task<ActionResult> List(int? day = null, int? month = null)
        {
            if (!day.HasValue || !month.HasValue)
            {
                day = DateTime.Today.Day;
                month = DateTime.Today.Month;

                // we don't redirect in order not to cvonfuse google. google will still have cannonical.
                //return RedirectToAction(nameof(List), new { day = DateTime.Today.Day, month = DateTime.Today.Month })
            }

            string cacheKey = string.Format("Events_Day_{0}_Month_{1}", day, month);
            EventListViewModel resultsFromCache = CacheHelper.GetFromCache<EventListViewModel>(cacheKey);

            if (resultsFromCache != null)
            {
                return View(resultsFromCache);
            }

            if (!TryCreateDateTime(day.Value, month.Value, out DateTime dateToGetEventsFor))
            {
                return NotFound();
            }

            List<EventViewModel> allEvents = await GetEventsForADay(dateToGetEventsFor);
            EventListViewModel result = new EventListViewModel
            {
                DateRequested = dateToGetEventsFor,
                Events = allEvents
            };

            if (allEvents != null && allEvents.Count > 0)
            {
                CacheHelper.AddToCache(cacheKey, result, DateTimeOffset.UtcNow.AddMinutes(HowLongToKeepInCacheMinutes));
            }

            return View(result);
        }        

        #endregion

        #region On This Day

        public async Task<PartialViewResult> OnThisDay() // TODO: ask in JS which day on the client is it?
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            DateTime today = DateTime.Today;
            string cacheKey = string.Format("OnThisDay_{0:yyyy-MM-dd}", today);
            List<EventViewModel> resultsFromCache = CacheHelper.GetFromCache<List<EventViewModel>>(cacheKey);

            if (resultsFromCache != null)
            {
                return PartialView("Partials/_OnThisDayPartial", resultsFromCache);
            }

            List<EventViewModel> allEvents = await GetEventsForADay(today);

            if (allEvents != null && allEvents.Count > 0)
            {
                var expires = new DateTimeOffset(today)
                    .AddDays(1)
                    .AddSeconds(1);

                CacheHelper.AddToCache(cacheKey, allEvents, expires);
            }

            return PartialView("Partials/_OnThisDayPartial", allEvents);
        }

        #endregion

        #region Private Methods

        private bool TryCreateDateTime(int day, int month, out DateTime dateToGetEventsFor)
        {
            dateToGetEventsFor = DateTime.Today;
            if (day > 31 || day < 1 || month < 1 || month > 12)
            {
                return false;
            }

            try
            {
                dateToGetEventsFor = new DateTime(DateTime.Today.Year, month, day);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<EventViewModel> GetEntityAndLoadViewModelAsync(string slug)
        {
            var eventInfo = await _Service.FindBySlugAsync(slug);

            if (eventInfo == null || eventInfo.Status != Core.ContentStatus.Published)
            {
                return null;
            }

            var result = new EventViewModel();
            EventMapper.TransferToViewModel(eventInfo, result);
            return result;
        }

        private async Task<List<EventViewModel>> GetEventsForADay(DateTime today)
        {
            var posts = await _Service.GetRecurringEventsAsync(today);

            if (posts == null)
            {
                return new List<EventViewModel>();
            }

            List<EventViewModel> result = new List<EventViewModel>();

            foreach (var item in posts)
            {
                EventViewModel viewModel = new EventViewModel();
                TransferFromEntityToViewModel(item, viewModel);

                result.Add(viewModel);
            }

            return result;
        }

        private void TransferFromEntityToViewModel(Event item, EventViewModel viewModel)
        {
            EventMapper.TransferToViewModel(item, viewModel);
        }

        #endregion
    }
}