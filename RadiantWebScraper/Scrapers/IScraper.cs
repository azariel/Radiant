using System.Collections.Generic;
using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.WebScraper.Scrapers
{
    internal interface IScraper
    {
        void GetTargetValueFromUrl(SupportedBrowser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aParserItems);
    }
}
