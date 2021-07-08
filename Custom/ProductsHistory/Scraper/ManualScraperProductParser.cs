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
        [JsonConverter(typeof(StringEnumConverter))]
        public ProductParserItemTarget Target { get; set; } = ProductParserItemTarget.Price;

        /// <summary>
        /// When we have the value, fetched from ManualScraperSequenceItems, apply this to find value
        /// </summary>
        public ProductDOMParserItem ValueParser { get; set; } = new();
    }
}
