using System.Collections.Generic;
using Radiant.Custom.ProductsHistory.Parsers;

namespace Radiant.Custom.ProductsHistory.Configuration
{
    public class ProductsHistoryConfiguration
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public List<ProductDOMParserItem> DOMParserItems { get; set; }
        //    = new()
        //{
        //    // Default parsers from most common shops
        //    new DOMParserItem
        //    {
        //        IfUrlContains = "amazon.c",
        //        RegexPattern = @"priceblock_ourprice(.*?)<\/span>",
        //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
        //        SubParserToExecuteOnTargetValue = new DOMParserItem
        //        {
        //            IfUrlContains = "amazon.c",
        //            RegexPattern = @"[\d,]+\.\d+",
        //            Target = DOMParserItem.DOMParserItemResultTarget.Value
        //        }
        //    }, new DOMParserItem
        //    {
        //        IfUrlContains = "newegg.c",
        //        RegexPattern = @"\""price\"":\""(.*?)[\""|,]",
        //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
        //        SubParserToExecuteOnTargetValue = new DOMParserItem
        //        {
        //            IfUrlContains = "newegg.c",
        //            RegexPattern = @"[\d,]+\.\d+",
        //            Target = DOMParserItem.DOMParserItemResultTarget.Value
        //        }
        //    }, new DOMParserItem
        //    {
        //        IfUrlContains = "bestbuy.c",
        //        RegexPattern = @"priceblock_ourprice(.*?)<\/span>",
        //        Target = DOMParserItem.DOMParserItemResultTarget.Value,
        //        SubParserToExecuteOnTargetValue = new DOMParserItem
        //        {
        //            IfUrlContains = "bestbuy.c",
        //            RegexPattern = @"[\d,]+\.\d+",
        //            Target = DOMParserItem.DOMParserItemResultTarget.Value
        //        }
        //    }
        //};
    }
}
