using System;
using System.Text.RegularExpressions;

namespace Radiant.Common.Utils
{
    public static class RegexUtils
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public static string GetWebSiteDomain(string aCompleteUrl)
        {
            if (string.IsNullOrWhiteSpace(aCompleteUrl))
                return "";

            return Regex.Match(aCompleteUrl, "^(?:https?:\\/\\/)?(?:[^@\\n]+@)?(?:www\\.)?([^:\\/\\n?]+)").Value
                .Replace("http://www.", "", StringComparison.InvariantCultureIgnoreCase)
                .Replace("https://www.", "", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
