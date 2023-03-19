using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Dmc.Cms.Web
{
    public static class StringExtensions
    {
        private static readonly char[] _MeaningfulPartSeparators = new char[] { ' ', '.', ',', ':', ';', '!', '?' };

        public static string StripTags(this string toStrip)
        {
            return Regex.Replace(toStrip, @"<(.|\n)*?>", string.Empty);
        }

        /// <summary>
        /// Converts multiple spaces to a single space
        /// </summary>
        /// <param name="value">Text in which to replace white space</param>
        /// <returns>Altered text</returns>
        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        /// <summary>
        /// Shortens the string to a meaningful part. Searches for word boundaries close to the desired lenght.
        /// </summary>
        /// <param name="lenght">Desired string lenght</param>
        /// <returns>Tries to shorten the string to the meaningful word boundary.</returns>
        public static string ShortenToClosestMeaningfulPart(this string toShorten, int lenght)
        {
            toShorten = (toShorten ?? "").Trim();

            if (toShorten.Length < lenght)
            {
                return toShorten;
            }

            int index = toShorten.LastIndexOfAny(_MeaningfulPartSeparators, lenght);

            if (index > (lenght / 2))
            {
                return toShorten.Substring(0, index);
            }

            return toShorten.Substring(0, lenght);
        }

        /// <summary>
        /// Replaces first occurence of the specified string
        /// </summary>
        /// <param name="text">Text in which to search for the instance</param>
        /// <param name="search">Text to search for</param>
        /// <param name="replace">Replacement text</param>
        /// <returns>Altered text</returns>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Replaces last occurence of the specified string
        /// </summary>
        /// <param name="text">Text in which to search for the instance</param>
        /// <param name="search">Text to search for</param>
        /// <param name="replace">Replacement text</param>
        /// <returns>Altered text</returns>
        public static string ReplaceLast(this string text, string search, string replace)
        {
            int pos = text.LastIndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
    }
}