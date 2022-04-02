using Radiant.WebScraper.Parsers.DOM;
using Radiant.WebScraper.Scrapers;
using System.Collections.Generic;

namespace Radiant.WebScraper.Business.Objects.TargetScraper
{
    public interface IScraperTarget
    {
        void Evaluate(Browser aSupportedBrowser, string aUrl, bool aAllowManualOperations, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aDOMParserItems);
    }
}
