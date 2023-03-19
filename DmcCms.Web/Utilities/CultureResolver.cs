using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace Dmc.Cms.Web
{
    public static class CultureResolver
    {
        //TODO: allowed

        public static void TrySetCultureFromRequest(bool setUICulture = true)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HttpRequest request = HttpContext.Current.Request;

            if (request.UserLanguages == null)
            {
                return;
            }

            string cultureString = request.UserLanguages[0];

            if (string.IsNullOrEmpty(cultureString))
            {
                return;
            }

            try
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(cultureString);
                Thread.CurrentThread.CurrentCulture = culture;

                if (setUICulture)
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
            catch (Exception) //TODO: log
            {
            }
        }
    }
}