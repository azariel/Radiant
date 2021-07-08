using System.Collections.Generic;
using Radiant.WebScraper.Scrapers.Conditions;

namespace Radiant.WebScraper.Scrapers.Manual
{
    public class ManualScraperItemParser
    {
        // ********************************************************************
        //                            Properties
        // ********************************************************************
        /// <summary>
        /// Check conditions (Certain manual scraper requires something to work. ex: if "Deal Price:" is found on the webpage, we
        /// can use a certain manual scraper
        /// </summary>
        public IScraperCondition Condition { get; set; }

        public string IfUrlContains { get; set; }
        public List<ManualScraperSequenceItem> ManualScraperSequenceItems { get; set; } = new();
    }
}
