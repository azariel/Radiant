using System.Collections.Generic;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;

namespace Radiant.WebScraper.Business.Objects.TargetScraper.Automatic.Selenium
{
    public class SeleniumDOMTargetScraper : IScraperTarget
    {
        // ********************************************************************
        //                            Public
        // ********************************************************************
        public void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems)
        {
            //String pageSource = driver.getPageSource();
        }

        // ********************************************************************
        //                            Properties
        // ********************************************************************
        public string DOM { get; set; }
    }
}
