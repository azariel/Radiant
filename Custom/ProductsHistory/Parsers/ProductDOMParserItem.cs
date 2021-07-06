using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.WebScraper.Parsers.DOM;

namespace Radiant.Custom.ProductsHistory.Parsers
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
