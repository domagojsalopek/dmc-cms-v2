using Dmc.Cms.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dmc.Cms.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleKeys.Admin)]
    public class DefaultController : Controller
    {
        #region Private Fields



        #endregion

        #region Constructors

        public DefaultController()
        {

        }

        #endregion

        #region Web Methods

        public ActionResult Index()
        {
            return View();
        }

        #endregion

        #region Menu Helper

        [ChildActionOnly]
        public PartialViewResult AdminMenu() // >This legacy code is such as hsit ... but i don't feel like chaning this
        {
            RouteData route = RouteTable.Routes.GetRouteData(HttpContext);
            UrlHelper urlHelper = new UrlHelper(new RequestContext(HttpContext, route));

            var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;
            string controllerName = routeValueDictionary["controller"].ToString().ToLower();
            string actionName = routeValueDictionary["action"].ToString().ToLower();

            ViewBag.Controller = controllerName;
            ViewBag.ActionName = actionName;

            return PartialView("_AdminMenuPartial");
        }

        #endregion
    }
}