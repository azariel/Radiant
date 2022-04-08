using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Radiant.WebScraper.Scrapers.Manual
{
    public class ManualScraperSequenceItemByClipboard : ManualScraperSequenceItem
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum ClipboardOperation
        {
            Get,
            Set
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        [JsonConverter(typeof(StringEnumConverter))]
        public ClipboardOperation Operation { get; set; }

        public string Value { get; set; }
    }
}
