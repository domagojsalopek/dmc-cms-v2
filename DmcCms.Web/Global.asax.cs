using Dmc.Cms.App;
using Dmc.Cms.Repository.Ef;
using Dmc.Core.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Net;
using Dmc.Cms.Web.Settings;
using Dmc.Cms.App.Services;

namespace Dmc.Cms.Web
{
    // quick & dirty
    public class MyRequireHttpsAttribute : RequireHttpsAttribute
    {
        const string _Https = "https://";
        const string _Get = "GET";

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!filterContext.HttpContext.Request.IsSecureConnection && IsGetRequest(filterContext))
            {
                string url = string.Concat(_Https, filterContext.HttpContext.Request.Url.Host, filterContext.HttpContext.Request.RawUrl);
                filterContext.Result = new RedirectResult(url, true);
            }
        }

        private bool IsGetRequest(AuthorizationContext filterContext)
        {
            return _Get.Equals(filterContext.HttpContext.Request.HttpMethod, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            //CultureResolver.TrySetCultureFromRequest();
        }

        protected void Application_Start()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            UrlRewrite.RewriteModule.Initialize();

#if DEBUG
            using (CmsContext context = new CmsContext())
            {
                context.Database.Initialize(false);
            }
#endif

            AreaRegistration.RegisterAllAreas();

            // Reset ViewEngines
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            // Normal MVC Configuration
//#if !DEBUG
//GlobalFilters.Filters.Add(new MyRequireHttpsAttribute());
//#endif
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            AppConfig.Configure(CreateConfigService); // should have configuration store

            // Dependency Injection. This is shit.
            var container = new DependencyInjectionContainer();
            DependencyConfiguration.Configure(container);
            ControllerBuilder.Current.SetControllerFactory(new CmsControllerFactory(container));

            // web api things
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WebApiControllerFactory(container));
        }
        

        private IConfigService CreateConfigService()
        {
            return new ConfigService(new CmsUnitOfWork(new CmsContext()));
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();
        }
    }
}
