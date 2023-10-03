using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Parsers
{
    public class ProductDOMParserItem : DOMParserItem
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductParserItemTarget ParserItemTarget { get; set; }
    }
}
