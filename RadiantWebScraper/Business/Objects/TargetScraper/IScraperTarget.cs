using System.Collections.Generic;
using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers.Manual;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    public interface IScraperTarget
    {
        void Evaluate(SupportedBrowser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<ManualScraperItemParser> aManualScraperItems, List<DOMParserItem> aDOMParserItems);
    }
}
