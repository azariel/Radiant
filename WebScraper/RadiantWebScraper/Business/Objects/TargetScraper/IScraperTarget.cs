using System.Collections.Generic;
using Radiant.WebScraper.RadiantWebScraper.Parsers.DOM;
using Radiant.WebScraper.RadiantWebScraper.Scrapers;

namespace Radiant.WebScraper.RadiantWebScraper.Business.Objects.TargetScraper
{
    public interface IScraperTarget
    {
        void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems);
    }
}
