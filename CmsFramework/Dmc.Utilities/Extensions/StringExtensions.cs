using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dmc.Utilities
{
    public static class StringExtensions
    {
        #region Private Fields

        private static readonly Regex _FinalSlugifyRegex = new Regex(@"[-]{2,}", RegexOptions.Compiled);

        #endregion

        #region Static Methods

        /// <summary>
        /// Strips HTML tags
        /// </summary>
        /// <param name="toStrip">Text from which to strip HTML tags</param>
        /// <returns>Cleaned string</returns>
        public static string StripTags(this string toStrip)
        {
            return Regex.Replace(toStrip, @"<(.|\n)*?>", String.Empty);
        }

        /// <summary>
        /// First converts multiple whitespaces to single and then trims
        /// </summary>
        /// <param name="str">string to trim</param>
        /// <returns>Reduced and trimmed string</returns>
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
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
            return ShortenToClosestMeaningfulPart(toShorten, lenght, new char[] { ' ', '.', ',', ':', ';', '!', '?' });
        }

        /// <summary>
        /// Shortens the string to a meaningful part. Searches for word boundaries close to the desired lenght.
        /// </summary>
        /// <param name="lenght">Desired string lenght</param>
        /// <param name="delimiters">Delimiters which indicate word boundaries</param>
        /// <returns>Tries to shorten the string to the meaningful word boundary.</returns>
        public static string ShortenToClosestMeaningfulPart(this string toShorten, int lenght, char[] delimiters)
        {
            if (string.IsNullOrWhiteSpace(toShorten))
            {
                return toShorten;
            }

            toShorten = toShorten.Trim();

            if (toShorten.Length < lenght)
            {
                return toShorten;
            }
            
            int index = toShorten.LastIndexOfAny(delimiters, lenght);

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
            //return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
            return string.Concat(text.Substring(0, pos), replace, text.Substring(pos + search.Length));
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
            return string.Concat(text.Substring(0, pos), replace, text.Substring(pos + search.Length)); //text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        /// <summary>
        /// Slugifies the input string for usage in URLs
        /// </summary>
        /// <param name="text">String to slugify</param>
        /// <returns>Slugified text</returns>
        public static string Slugify(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            text = text.Trim();

            StringBuilder slugBuilder = new StringBuilder();

            foreach (char c in text) // TODO: More
            {
                switch (c)
                {
                    case ' ':
                        slugBuilder.Append('-');
                        break;
                    case '&':
                        slugBuilder.Append("and");
                        break;
                    default:
                        if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                        {
                            slugBuilder.Append(c);
                        }
                        else
                        {
                            slugBuilder.Append('-');
                        }
                        break;
                }
            }

            string finalString = slugBuilder.ToString().ToLower();
            return _FinalSlugifyRegex.Replace(finalString, "-");
        }

        #endregion
    }
}
