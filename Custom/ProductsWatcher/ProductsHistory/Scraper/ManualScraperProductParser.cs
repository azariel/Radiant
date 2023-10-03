using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.Custom.ProductsWatcher.ProductsHistory.Parsers;
using Radiant.WebScraper.RadiantWebScraper.Scrapers.Manual;

namespace Radiant.Custom.ProductsWatcher.ProductsHistory.Scraper
{
    public class ManualScraperProductParser : ManualScraperItemParser
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductParserItemTarget Target { get; set; } = ProductParserItemTarget.Price;

        /// <summary>
        /// When we have the value, fetched from ManualScraperSequenceItems, apply this to find value
        /// </summary>
        public ProductDOMParserItem ValueParser { get; set; } = new();
    }
}
