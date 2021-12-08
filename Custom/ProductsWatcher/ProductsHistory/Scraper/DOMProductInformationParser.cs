using System;
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
        internal static string ParseTitle(string aUrl, string aDOM, List<ProductDOMParserItem> aDOMParserItems)
        {
            if (string.IsNullOrWhiteSpace(aUrl) || string.IsNullOrWhiteSpace(aDOM) || aDOMParserItems == null || aDOMParserItems.Count <= 0)
                return null;

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Title))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                if (_Value != null)
                    return _Value;
            }

            LoggingManager.LogToFile("84FF8277-1D1F-4EEE-808D-4922A99C35CA", $"Couldn't find title in DOM using [{aDOMParserItems.Count}] DOM parsers for Url [{aUrl}].");
            return null;
        }

        internal static double? ParseDouble(string aUrl, string aDOM, ProductDOMParserItem[] aDOMParserItems)
        {
            if (string.IsNullOrWhiteSpace(aUrl) || string.IsNullOrWhiteSpace(aDOM) || aDOMParserItems == null || aDOMParserItems.Length <= 0)
                return null;

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => aUrl.Contains(w.IfUrlContains, StringComparison.InvariantCultureIgnoreCase)))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                if (_Value != null && double.TryParse(_Value, out double _DoubleValue))
                    return _DoubleValue;
            }

            LoggingManager.LogToFile("413486D0-7DA3-4D3C-A203-432CCA1A3A55", $"Couldn't find target value in DOM using [{aDOMParserItems.Length}] DOM parsers for Url [{aUrl}].");
            return null;
        }

        public static bool? ParseBoolean(string aUrl, string aDOM, ProductDOMParserItem[] aDOMParserItems)
        {
            if (string.IsNullOrWhiteSpace(aUrl) || string.IsNullOrWhiteSpace(aDOM) || aDOMParserItems == null || aDOMParserItems.Length <= 0)
                return null;

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => aUrl.Contains(w.IfUrlContains, StringComparison.InvariantCultureIgnoreCase)))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                return !string.IsNullOrWhiteSpace(_Value);
            }

            LoggingManager.LogToFile("F80BCA19-6B8F-4C20-9B6D-5D63A68CDD57", $"Couldn't find target value in DOM using [{aDOMParserItems.Length}] DOM parsers for Url [{aUrl}].");
            return null;
        }
    }
}
