using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.Custom.ProductsHistory.Parsers;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.Custom.ProductsHistory.Scraper
{
    public class ManualScraperProductParser : ManualScraperItemParser
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string IfUrlContains { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductParserItemTarget Target { get; set; } = ProductParserItemTarget.Price;
        public ProductDOMParserItem ValueParser { get; set; } = new();// When we have the value, fetched from ManualScraperSequenceItems, apply this to find value
    }
}
