using Radiant.WebScraper.Business.Objects.TargetScraper;
using Radiant.WebScraper.Parsers.DOM;
using System.Collections.Generic;

namespace Radiant.WebScraper.Scrapers
{
    internal interface IScraper
    {
        void GetTargetValueFromUrl(Browser aSupportedBrowser, string aUrl, IScraperTarget aTarget, List<IScraperItemParser> aScraperItemsParser, List<DOMParserItem> aParserItems);
    }
}
