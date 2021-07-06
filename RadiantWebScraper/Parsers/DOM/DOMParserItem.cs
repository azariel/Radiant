using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Radiant.WebScraper.Parsers.DOM
{
    public class DOMParserItem
    {
        // ********************************************************************
        //                            Nested
        // ********************************************************************
        public enum DOMParserItemResultTarget
        {
            Value,
            Group0Value,
            Group1Value
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Parser will be applied if Url contains following case insensitive text ex: amazon.com
        /// </summary>
        public string IfUrlContains { get; set; }

        public string RegexPattern { get; set; }
        public DOMParserItem SubParserToExecuteOnTargetValue { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DOMParserItemResultTarget Target { get; set; } = DOMParserItemResultTarget.Value;
        public string UID { get; set; } = Guid.NewGuid().ToString();
    }
}
