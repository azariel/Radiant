using System.Collections.Generic;
using System.Linq;
using Radiant.Common.Diagnostics;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.WebScraper.Parsers.DOM;

namespace Radiant.Custom.ProductsHistory.Scraper
{
    internal static class DOMProductInformationParser
    {
        // ********************************************************************
        //                            Internal
        // ********************************************************************
        internal static double? ParsePrice(string aUrl, string aDOM, List<ProductDOMParserItem> aDOMParserItems)
        {
            if (string.IsNullOrWhiteSpace(aUrl) || string.IsNullOrWhiteSpace(aDOM) || aDOMParserItems == null || aDOMParserItems.Count <= 0)
                return null;

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => w.ParserItemTarget == ProductDOMParserItem.ProductDOMParserItemTarget.Price))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                if (_Value != null && double.TryParse(_Value, out double _Price))
                    return _Price;
            }

            LoggingManager.LogToFile($"Couldn't find price in DOM using [{aDOMParserItems.Count}] DOM parsers for Url [{aUrl}].");
            return null;
        }

        public static string ParseTitle(string aUrl, string aDOM, List<ProductDOMParserItem> aDOMParserItems)
        {
            if (string.IsNullOrWhiteSpace(aUrl) || string.IsNullOrWhiteSpace(aDOM) || aDOMParserItems == null || aDOMParserItems.Count <= 0)
                return null;

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => w.ParserItemTarget == ProductDOMParserItem.ProductDOMParserItemTarget.Title))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                if (_Value != null)
                    return _Value;
            }

            LoggingManager.LogToFile($"Couldn't find title in DOM using [{aDOMParserItems.Count}] DOM parsers for Url [{aUrl}].");
            return null;
        }
    }
}
