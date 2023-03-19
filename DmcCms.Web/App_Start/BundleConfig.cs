using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Dmc.Cms.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            // FrondEnd
            RegisterFrontendBundles(bundles);

            // Administration
            RegisterAdministrationBundles(bundles);

            // always
            BundleTable.EnableOptimizations = true;
        }

        #region Administration

        private static void RegisterAdministrationBundles(BundleCollection bundles)
        {
            AddAdminCSS(bundles);
            AddAdminJS(bundles);
        }

        //TODO: maybe different paths, inisde admina rea ... 
        private static void AddAdminCSS(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/resources/admincss")
            {
                Orderer = new NonOrderingOrderer()
            }
            .Include(
                "~/AdminResources/css/bootstrap.min.css",
                "~/AdminResources/css/bootstrap-theme.min.css",
                "~/AdminResources/css/admin.css"));
        }

        private static void AddAdminJS(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/adminjs")
            {
                Orderer = new NonOrderingOrderer()
            }
            .Include(
                "~/AdminResources/js/jquery.validate*",
                "~/AdminResources/js/jquery.unobtrusive*",
                "~/AdminResources/js/bootstrap*"));
        }

        #endregion

        #region Frontend

        private static void RegisterFrontendBundles(BundleCollection bundles)
        {
            AddFrontEndCSS(bundles);
            AddFrontEndJS(bundles);
        }

        private static void AddFrontEndCSS(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/resources/style")
            {
                Orderer = new NonOrderingOrderer()
            }
            .Include(
                "~/resources/css/fonts-and-bootstrap.min.css",
                "~/resources/style.min.css"));
            //.Include(
            //    "~/resources/css/fonts-local.css",
            //    "~/resources/css/bootstrap.css",
            //    "~/resources/style.css"));

            bundles.Add(new StyleBundle("~/resources/css/style")
            {
                Orderer = new NonOrderingOrderer()
            }
            .Include(
                "~/resources/css/main-style-bundle.min.css",
                
                // we no longer minify these two eagerly. we need to be able to update only them.
                "~/resources/css/colors.css", 
                "~/resources/css/custom.css"
            ));
            //.Include(
            //    "~/resources/css/dark.css",
            //    "~/resources/css/font-icons.css",
            //    "~/resources/css/animate.css",
            //    "~/resources/css/magnific-popup.css",
            //    "~/resources/css/colors.css",
            //    "~/resources/css/custom.css",
            //    "~/resources/css/fonts.css",
            //    "~/resources/css/sharebuttons.css",
            //    "~/resources/css/yt.css",
            //    //"~/resources/highlight/styles/vs.css"
            //    //"~/resources/css/prism.css",
            //    "~/resources/css/prism/themes/prism-vs.css",
            //    "~/resources/css/responsive.css"
            //    ));
        }

        private static void AddFrontEndJS(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts/jquery", "https://code.jquery.com/jquery-3.4.1.min.js")
                            .Include("~/resources/js/jquery.js"));

            bundles.Add(new ScriptBundle("~/scripts/knockout", "https://cdnjs.cloudflare.com/ajax/libs/knockout/3.5.0/knockout-min.js")
                            .Include("~/resources/js/knockout-min.js"));

            bundles.Add(new ScriptBundle("~/scripts/js")
            {
                Orderer = new NonOrderingOrderer()
            }
            .Include(
                "~/resources/js/scripts-bundle.min.js"
            ));
        }

        #endregion
    }
}