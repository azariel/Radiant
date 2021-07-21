﻿using System.Collections.Generic;
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

            foreach (ProductDOMParserItem _ParserItem in aDOMParserItems.Where(w => w.ParserItemTarget == ProductParserItemTarget.Price))
            {
                string _Value = DOMParserExecutor.Execute(aUrl, aDOM, _ParserItem);

                if (_Value != null && double.TryParse(_Value, out double _Price))
                    return _Price;
            }

            LoggingManager.LogToFile("A22DF9B4-66B7-4157-BFEA-D9F77F35CC14", $"Couldn't find price in DOM using [{aDOMParserItems.Count}] DOM parsers for Url [{aUrl}].");
            return null;
        }

        public static string ParseTitle(string aUrl, string aDOM, List<ProductDOMParserItem> aDOMParserItems)
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
    }
}