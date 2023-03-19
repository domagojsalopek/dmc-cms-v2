using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dmc.Cms.Web
{
    public class RouteConfig
    {
        private const string ControllersNameSpace = "Dmc.Cms.Web.Controllers";

        public static void RegisterRoutes(RouteCollection routes) // now we'll just have manually.
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "PostsByTagFirstPage",
                "tag/{tagName}",
                new { controller = "default", action = "postsbytag", page = 1 },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "PostsByTag",
                "tag/{tagName}/page/{page}",
                new { controller = "default", action = "postsbytag", page = UrlParameter.Optional },
                new { page = @"\d+" },
                new[] { ControllersNameSpace }
            );            

            routes.MapRoute(
                "PostsByCategoryFirstPage",
                "category/{categoryName}",
                new { controller = "default", action = "postsbycategory", page = 1 },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "PostsByCategory",
                "category/{categoryName}/page/{page}",
                new { controller = "default", action = "postsbycategory", page = UrlParameter.Optional },
                new { page = @"\d+" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "EventList",
                "events/{day}-{month}",
                new { controller = "event", action = "list" },
                //new { day = @"\d+", month = @"\d+" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "EventListForToday",
                "events",
                new { controller = "event", action = "list" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "EventDetails",
                "events/{day}-{month}/{slug}",
                new { controller = "event", action = "details" },
                new { day = @"\d+", month = @"\d+" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "PostDetails",
                "post/{slug}",
                new { controller = "default", action = "details" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                 "BlankHome",
                 "",
                 new { controller = "default", action = "index", page = 1 },
                 new[] { ControllersNameSpace }
             );

            routes.MapRoute(
                "IndexPagination",
                "page/{page}",
                new { controller = "default", action = "index", page = UrlParameter.Optional },
                new { page = @"\d+" },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "PageDetails",
                "page/{slug}",
                new { controller = "page", action = "details", page = UrlParameter.Optional },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "default", action = "index", id = UrlParameter.Optional },
                new[] { ControllersNameSpace }
            );

            routes.MapRoute(
                "Default_Pagination",
                "{controller}/page/{page}",
                new { controller = "default", action = "index", page = 1 },
                new[] { ControllersNameSpace }
            );
        }
    }
}
