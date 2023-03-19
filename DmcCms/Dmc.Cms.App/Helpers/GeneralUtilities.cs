using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dmc.Cms.App.Helpers
{
    public static class GeneralUtilities
    {
        #region Public Methods

        public static string Slugify(string input)
        {
            input = (input ?? "").Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            StringBuilder url = new StringBuilder();

            foreach (char c in input)
            {
                switch (c)
                {
                    case ' ':
                        url.Append('-');
                        break;

                    case '&':
                        url.Append("and");
                        break;

                    default:
                        if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                        {
                            url.Append(c);
                        }
                        else
                        {
                            url.Append('-');
                        }
                        break;
                }
            }

            string urlString = url.ToString().ToLower();

            // replace multple instances of - with one
            urlString = Regex.Replace(urlString, @"[-]{2,}", "-", RegexOptions.None);

            return urlString;
        }

        #endregion
    }
}
