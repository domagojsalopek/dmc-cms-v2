using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Utilities
{
    public static class DmcConvert
    {
        public static int? ToInt32(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (int.TryParse(input, out int valueToReturn))
            {
                return valueToReturn;
            }

            return null;
        }

        public static int ToInt32(string input, int defaultValueIfParsingFailed)
        {
            int? convertResult = ToInt32(input);

            if (convertResult.HasValue)
            {
                return convertResult.Value;
            }

            return defaultValueIfParsingFailed;
        }

        public static bool? ToBool(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            if (bool.TryParse(input, out bool valueToReturn))
            {
                return valueToReturn;
            }

            return null;
        }

        public static bool ToBool(string input, bool defaultValueIfParsingFailed)
        {
            bool? convertResult = ToBool(input);

            if (convertResult.HasValue)
            {
                return convertResult.Value;
            }

            return defaultValueIfParsingFailed;
        }
    }
}
