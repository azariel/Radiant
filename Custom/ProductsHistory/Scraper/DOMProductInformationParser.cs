using System.Text.RegularExpressions;

namespace Radiant.Custom.ProductsHistory.Scraper
{
    internal static class DOMProductInformationParser
    {
        // ********************************************************************
        //                            Internal
        // ********************************************************************
        internal static double? ParsePrice(string aDOM)
        {
            if (string.IsNullOrWhiteSpace(aDOM))
                return null;

            Regex _PriceBlockOurPriceRegex = new Regex(@"priceblock_ourprice(.*?)<\/span>");

            var _MatchResult = _PriceBlockOurPriceRegex.Match(aDOM);
            if (!_MatchResult.Success)
                return null;

            Regex _PriceRegex = new Regex(@"[\d,]+\.\d+");

            var _PriceMatchResult = _PriceRegex.Match(_MatchResult.Value);
            if (!_PriceMatchResult.Success)
                return null;

            string _PriceAsString = _PriceMatchResult.Value;

            if (!double.TryParse(_PriceAsString, out double _Price))
                return null;

            return _Price;
        }

        public static string ParseTitle(string aDOM)
        {
            if (string.IsNullOrWhiteSpace(aDOM))
                return null;

            Regex _TitleRegex = new Regex(@"<title>(.*?)<\/title>");

            var _MatchResult = _TitleRegex.Match(aDOM);
            if (!_MatchResult.Success || _MatchResult.Groups.Count < 2)
                return null;

            return _MatchResult.Groups[1].Value;
        }
    }
}
