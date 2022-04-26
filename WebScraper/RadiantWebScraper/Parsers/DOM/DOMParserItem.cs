using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Radiant.Common.Business;
using Radiant.WebScraper.Scrapers.Conditions;

namespace Radiant.WebScraper.Parsers.DOM
{
    public class DOMParserItem
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Parser will be applied if Url contains following case insensitive text ex: amazon.com
        /// </summary>
        public string IfUrlContains { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RegexItemResultMatch RegexMatch { get; set; } = RegexItemResultMatch.First;

        public string RegexPattern { get; set; }
        public DOMParserItem SubParserToExecuteOnTargetValue { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RegexItemResultTarget Target { get; set; } = RegexItemResultTarget.Value;

        public string UID { get; set; } = Guid.NewGuid().ToString();

        public StringValueCondition ValueCondition { get; set; }
    }
}
